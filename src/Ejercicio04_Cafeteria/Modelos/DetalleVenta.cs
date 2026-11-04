using System;
using System.Collections.Generic;
using System.Text;

namespace Ejercicio04_Cafeteria.Modelos
{
    public class DetalleVenta
    {
        public int ProductoId { get; set; }
        public string Tipo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
