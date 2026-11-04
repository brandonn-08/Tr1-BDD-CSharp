using Ejercicio04_Cafeteria.Persistencia;

namespace Ejercicio04_Cafeteria.Reportes
{
    public class ReporteVentas
    {
        public IEnumerable<(string Nombre, int Cantidad, decimal Total)> Obtener(DateTime inicio, DateTime fin)
        {
            if (inicio.Date > fin.Date) throw new ArgumentException("El período no es válido.");
            var platos = Archivo.Leer<Modelos.Plato>("platos.json").ToDictionary(x => x.Id);
            return Archivo.Leer<Modelos.Venta>("ventas.json").Where(v => v.Fecha >= inicio.Date && v.Fecha < fin.Date.AddDays(1)).SelectMany(v => v.Detalles.Where(d => d.Tipo.Equals("Plato", StringComparison.OrdinalIgnoreCase) || d.Tipo == "")).Where(d => platos.ContainsKey(d.ProductoId)).GroupBy(d => platos[d.ProductoId].Nombre).Select(g => (g.Key, g.Sum(x => x.Cantidad), g.Sum(x => x.Cantidad * x.Precio))).OrderByDescending(x => x.Item2);
        }
        public void Mostrar(DateTime inicio, DateTime fin) { foreach (var x in Obtener(inicio, fin)) Console.WriteLine($"{x.Nombre}: {x.Cantidad} - {x.Total:F2}"); }
    }
}
