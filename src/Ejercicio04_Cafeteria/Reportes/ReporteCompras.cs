using Ejercicio04_Cafeteria.Persistencia;
using Ejercicio04_Cafeteria.Modelos;

namespace Ejercicio04_Cafeteria.Reportes
{
    public class ReporteCompras
    {
        public List<Ingrediente> Obtener() => Archivo.Leer<Ingrediente>("ingredientes.json").Where(x => x.Stock < x.StockMinimo).ToList();
        public void Mostrar() { foreach (var x in Obtener()) Console.WriteLine($"{x.Nombre} ({x.Unidad}): actual {x.Stock:F2}, mínimo {x.StockMinimo:F2}, comprar {(x.StockMinimo - x.Stock):F2}"); }
    }
}
