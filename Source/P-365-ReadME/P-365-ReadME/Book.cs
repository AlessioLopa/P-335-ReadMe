using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P_365_ReadME
{
    class Book
    {
        List<object> books = new List<object>();

        public string Title { get; set}
        public string Description { get; set; }
        public string Author { get; set; }
        public int NumberOfPages { get; set; }
    }
}
