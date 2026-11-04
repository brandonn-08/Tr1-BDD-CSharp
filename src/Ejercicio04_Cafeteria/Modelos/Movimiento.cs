using System;
using System.Collections.Generic;
using System.Text;

namespace Ejercicio04_Cafeteria.Modelos
{
    public class Movimiento
    {
        public int Id { get; set; }
        public int IngredienteId { get; set; }
        public string Tipo { get; set; } = "";
        public decimal Cantidad { get; set; }
        public decimal StockAnterior { get; set; }
        public decimal StockNuevo { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = "";
    }
}
