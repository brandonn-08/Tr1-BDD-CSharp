using System;
using System.Collections.Generic;
using System.Text;

namespace Ejercicio04_Cafeteria.Modelos
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public List<DetalleVenta> Detalles { get; set; } = new();
        public decimal Total { get; set; }
    }
}