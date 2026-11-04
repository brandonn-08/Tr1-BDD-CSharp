using Ejercicio04_Cafeteria.Modelos;
using Ejercicio04_Cafeteria.Persistencia;

namespace Ejercicio04_Cafeteria.Servicios
{
    public class PlatoService
    {
        private const string Platos = "platos.json", Recetas = "recetas.json", Ingredientes = "ingredientes.json";
        public List<Plato> Listar() => Archivo.Leer<Plato>(Platos);
        public Plato? Buscar(int id) => Listar().FirstOrDefault(x => x.Id == id);
        public List<Receta> ListarRecetas() => Archivo.Leer<Receta>(Recetas);
        public Receta? ObtenerReceta(int platoId) => ListarRecetas().FirstOrDefault(x => x.PlatoId == platoId);
        public Plato Registrar(Plato plato)
        {
            if (string.IsNullOrWhiteSpace(plato.Nombre)) throw new ArgumentException("El nombre es obligatorio.");
            var l = Listar(); if (l.Any(x => x.Id == plato.Id)) throw new InvalidOperationException("Ya existe el plato.");
            plato.Id = plato.Id <= 0 ? (l.Count == 0 ? 1 : l.Max(x => x.Id) + 1) : plato.Id;
            l.Add(plato); Archivo.Guardar(Platos, l); return plato;
        }
        public Receta AsignarReceta(Receta receta)
        {
            if (Buscar(receta.PlatoId) is null || receta.Ingredientes.Count == 0 || receta.Ingredientes.Any(x => x.Cantidad <= 0)) throw new ArgumentException("La receta no es válida.");
            var ids = Archivo.Leer<Ingrediente>(Ingredientes).Select(x => x.Id).ToHashSet();
            if (receta.Ingredientes.Any(x => !ids.Contains(x.IngredienteId))) throw new ArgumentException("La receta contiene ingredientes inexistentes.");
            var l = ListarRecetas(); var actual = l.FindIndex(x => x.PlatoId == receta.PlatoId);
            receta.Id = actual >= 0 ? l[actual].Id : (l.Count == 0 ? 1 : l.Max(x => x.Id) + 1);
            if (actual >= 0) l[actual] = receta; else l.Add(receta); Archivo.Guardar(Recetas, l); return receta;
        }
        public bool EstaUsado(int ingredienteId) => ListarRecetas().Any(r => r.Ingredientes.Any(d => d.IngredienteId == ingredienteId));
        public decimal CalcularCosto(int platoId)
        {
            var receta = ObtenerReceta(platoId) ?? throw new KeyNotFoundException("El plato no tiene receta.");
            var ins = Archivo.Leer<Ingrediente>(Ingredientes).ToDictionary(x => x.Id);
            return receta.Ingredientes.Sum(x => x.Cantidad * (ins.TryGetValue(x.IngredienteId, out var i) ? i.Costo : throw new KeyNotFoundException("Ingrediente inexistente.")));
        }
        public decimal CalcularPrecio(int platoId) => CalcularCosto(platoId) * 2m;
    }
}
