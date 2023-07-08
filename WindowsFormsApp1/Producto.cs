using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class Producto
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public float precio { get; set; }
        public int cantidad { get; set; }
        public byte[] imagen { get; set; }
    }
}
