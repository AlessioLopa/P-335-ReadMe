using System.IO.Compression;
using System.Xml;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace P_365_ReadME;

public partial class ApiPage : ContentPage
{
    HttpClient client = new();
    bool useXml = false;

    public ObservableCollection<Book> Books { get; set; } = new ObservableCollection<Book>();

    public ApiPage()
    {
        InitializeComponent();
        BooksCollectionView.ItemsSource = Books;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Call API
            var response = await client.GetAsync(endpoint.Text);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var booksList = JsonConvert.DeserializeObject<List<Book>>(content);

                Books.Clear();
                foreach (var book in booksList)
                {
                    // Fetch the cover image for each book
                    var bookResponse = await client.GetAsync($"http://10.0.2.2:3000/epub/{book.Id}");
                    if (bookResponse.IsSuccessStatusCode)
                    {
                        var bookContent = bookResponse.Content;

                        // Open epub ZIP
                        ZipArchive archive = new ZipArchive(await bookContent.ReadAsStreamAsync());
                        var coverEntry = archive.GetEntry("OEBPS/Images/cover.png");
                        var coverStream = coverEntry.Open();

                        // Convert the stream to an ImageSource
                        book.CoverImage = ImageSource.FromStream(() => coverStream);

                        // Load CONTENT (meta data)
                        var contentString = new StreamReader(archive.GetEntry("OEBPS/content.opf").Open()).ReadToEnd();
                        if (useXml)
                        {
                            #region XML version
                            // load meta-data from xml
                            var xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(contentString);

                            // Retrieve the title element
                            XmlNode titleNode = xmlDoc.SelectSingleNode("//dc:title", GetNamespaceManager(xmlDoc));
                            book.Title = titleNode != null ? titleNode.InnerText : "not found with xml";
                            #endregion
                        }
                        else
                        {
                            #region plain text version
                            int start = contentString.IndexOf("<dc:title>") + 10;
                            int end = contentString.IndexOf("</dc:title>");
                            book.Title = (start != -1 && end != -1) ? contentString.Substring(start, end - start) : "Title node not found.";
                            #endregion
                        }

                        Books.Add(book);
                    }
                }
            }
            else
            {
                throw new Exception($"Bad status : {response.StatusCode}, {response.Headers},{response.Content}");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(ex.Message, ex.StackTrace, "ok");
        }
    }

    private static XmlNamespaceManager GetNamespaceManager(XmlDocument xmlDoc)
    {
        XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
        nsManager.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
        return nsManager;
    }

    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {
        useXml = e.Value;
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public ImageSource CoverImage { get; set; }
    }
}
