using Ejercicio04_Cafeteria.Modelos;
using Ejercicio04_Cafeteria.Servicios;

namespace Ejercicio04_Cafeteria.Reportes
{
    public class ReportePlatos
    {
        public void Mostrar(PlatoService servicio)
        {
            Console.WriteLine("Plato | Costo | Precio | Margen | Margen %");
            foreach (var p in servicio.Listar()) { var c = servicio.CalcularCosto(p.Id); var v = c * 2; Console.WriteLine($"{p.Nombre} | {c:F2} | {v:F2} | {(v - c):F2} | {(v == 0 ? 0 : (v - c) / v * 100):F2}%"); }
        }
    }
}
