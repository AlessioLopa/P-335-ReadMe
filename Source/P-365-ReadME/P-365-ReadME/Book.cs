using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P_365_ReadME
{
    public class Book
    {
        public int id { get; set; }
        public string? title { get; set; }
        public string? epub { get; set; }

        public override string ToString()
        {
            return $"[Book {id}";
        }
    }
}
