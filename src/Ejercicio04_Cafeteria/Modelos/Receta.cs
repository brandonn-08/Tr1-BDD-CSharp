using System;
using System.Collections.Generic;
using System.Text;

namespace Ejercicio04_Cafeteria.Modelos
{
    public class Receta
    {
        public int Id { get; set; }
        public int PlatoId { get; set; }
        public List<DetalleReceta> Ingredientes { get; set; } = new();
    }
}
