using System;
using System.Collections.Generic;
using System.Text;

namespace Ejercicio04_Cafeteria.Modelos
{
    public class Bebida
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}
