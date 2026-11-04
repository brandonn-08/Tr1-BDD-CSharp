using Ejercicio04_Cafeteria.Persistencia;
using Ejercicio04_Cafeteria.Modelos;

namespace Ejercicio04_Cafeteria.Reportes
{
    public class ReportePreparacion
    {
        public Dictionary<string, int> Obtener()
        {
            var ins = Archivo.Leer<Ingrediente>("ingredientes.json").ToDictionary(x => x.Id); var resultado = new Dictionary<string, int>();
            foreach (var p in Archivo.Leer<Plato>("platos.json")) { var r = Archivo.Leer<Receta>("recetas.json").FirstOrDefault(x => x.PlatoId == p.Id); if (r is null || r.Ingredientes.Count == 0) continue; var unidades = r.Ingredientes.Select(x => ins.TryGetValue(x.IngredienteId, out var i) && x.Cantidad > 0 ? (int)Math.Floor(i.Stock / x.Cantidad) : 0); resultado[p.Nombre] = unidades.Min(); }
            return resultado;
        }
        public void Mostrar() { foreach (var x in Obtener()) Console.WriteLine($"{x.Key}: {x.Value} unidad(es)"); }
    }
}
