using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P_365_ReadME
{
    class Book
    {

        public string Title { get; set}
        public string Description { get; set; }
        public string Author { get; set; }
        public int NumberOfPages { get; set; }

        List<object> books = new List<object>();

        
    }
}
