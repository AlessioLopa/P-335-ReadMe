namespace P_365_ReadME
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void OnImageButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BookPage());
        }
    }

}
