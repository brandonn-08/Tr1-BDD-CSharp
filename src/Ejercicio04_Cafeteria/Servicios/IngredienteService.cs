using Ejercicio04_Cafeteria.Modelos;
using Ejercicio04_Cafeteria.Persistencia;

namespace Ejercicio04_Cafeteria.Servicios
{
    public class IngredienteService
    {
        private const string ArchivoNombre = "ingredientes.json";
        private readonly PlatoService? platos;

        public IngredienteService(PlatoService? platos = null) => this.platos = platos;

        public List<Ingrediente> Listar() => Archivo.Leer<Ingrediente>(ArchivoNombre);
        public Ingrediente? Buscar(int id) => Listar().FirstOrDefault(x => x.Id == id);

        public Ingrediente Registrar(Ingrediente ingrediente)
        {
            Validar(ingrediente);
            var lista = Listar();
            if (lista.Any(x => x.Id == ingrediente.Id)) throw new InvalidOperationException("Ya existe el ingrediente.");
            ingrediente.Id = ingrediente.Id <= 0 ? SiguienteId(lista) : ingrediente.Id;
            lista.Add(ingrediente);
            Archivo.Guardar(ArchivoNombre, lista);
            return ingrediente;
        }

        public void Actualizar(Ingrediente ingrediente)
        {
            Validar(ingrediente);
            var lista = Listar();
            var i = lista.FindIndex(x => x.Id == ingrediente.Id);
            if (i < 0) throw new KeyNotFoundException("Ingrediente inexistente.");
            lista[i] = ingrediente;
            Archivo.Guardar(ArchivoNombre, lista);
        }

        public void Eliminar(int id)
        {
            if (platos?.EstaUsado(id) == true || (platos is null && Archivo.Leer<Receta>("recetas.json").Any(r => r.Ingredientes.Any(d => d.IngredienteId == id)))) throw new InvalidOperationException("No se puede eliminar un ingrediente usado en una receta.");
            var lista = Listar();
            if (lista.RemoveAll(x => x.Id == id) == 0) throw new KeyNotFoundException("Ingrediente inexistente.");
            Archivo.Guardar(ArchivoNombre, lista);
        }

        private static void Validar(Ingrediente x)
        {
            if (string.IsNullOrWhiteSpace(x.Nombre)) throw new ArgumentException("El nombre es obligatorio.");
            if (x.Unidad.Trim().ToLowerInvariant() is not ("kg" or "litros")) throw new ArgumentException("La unidad debe ser kg o litros.");
            if (x.Costo <= 0 || x.Stock < 0 || x.StockMinimo < 0) throw new ArgumentException("Costo, stock y mínimo deben ser válidos.");
            x.Unidad = x.Unidad.Trim().ToLowerInvariant();
        }
        private static int SiguienteId(List<Ingrediente> l) => l.Count == 0 ? 1 : l.Max(x => x.Id) + 1;
    }
}
