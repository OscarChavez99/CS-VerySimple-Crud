using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class Product
    {
        public int id { get; set; }
        public string name { get; set; }
        public float price { get; set; }
        public int stock { get; set; }
        public byte[] image { get; set; }
    }
}
