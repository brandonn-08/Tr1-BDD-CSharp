using System;
using System.Collections.Generic;
using System.Text;

namespace Ejercicio04_Cafeteria.Modelos
{
    public class Ingrediente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Unidad { get; set; } = "";
        public decimal Stock { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal Costo { get; set; }
    }
}
