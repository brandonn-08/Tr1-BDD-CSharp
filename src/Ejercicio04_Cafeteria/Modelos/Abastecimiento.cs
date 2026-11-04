using System;
using System.Collections.Generic;
using System.Text;

namespace Ejercicio04_Cafeteria.Modelos
{
    public class Abastecimiento
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string Tipo { get; set; } = "";
        public decimal Cantidad { get; set; }
        public decimal Costo { get; set; }
        public DateTime Fecha { get; set; }
    }
}
