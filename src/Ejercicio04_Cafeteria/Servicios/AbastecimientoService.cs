using Ejercicio04_Cafeteria.Modelos;
using Ejercicio04_Cafeteria.Persistencia;

namespace Ejercicio04_Cafeteria.Servicios
{
    public class AbastecimientoService
    {
        private const string ArchivoNombre = "abastecimientos.json";
        private readonly InventarioService inventario;
        public AbastecimientoService(InventarioService? inventario = null) => this.inventario = inventario ?? new InventarioService();
        public List<Abastecimiento> Listar() => Archivo.Leer<Abastecimiento>(ArchivoNombre);
        public Abastecimiento RegistrarIngrediente(Ingrediente ingrediente, decimal cantidad, decimal costo)
        {
            if (cantidad <= 0 || costo <= 0) throw new ArgumentException("Cantidad y costo deben ser positivos.");
            var stockAnterior = ingrediente.Stock;
            ingrediente.Costo = (stockAnterior * ingrediente.Costo + cantidad * costo) / (stockAnterior + cantidad);
            inventario.Entrada(ingrediente, cantidad);
            return Guardar(new Abastecimiento { ProductoId = ingrediente.Id, Tipo = "Ingrediente", Cantidad = cantidad, Costo = costo, Fecha = DateTime.Now });
        }
        public Abastecimiento RegistrarBebida(Bebida bebida, int cantidad, decimal costo)
        {
            if (cantidad <= 0 || costo <= 0) throw new ArgumentException("Cantidad y costo deben ser positivos.");
            bebida.Stock += cantidad;
            return Guardar(new Abastecimiento { ProductoId = bebida.Id, Tipo = "Bebida", Cantidad = cantidad, Costo = costo, Fecha = DateTime.Now });
        }
        private static Abastecimiento Guardar(Abastecimiento a) { var l = Archivo.Leer<Abastecimiento>(ArchivoNombre); a.Id = l.Count == 0 ? 1 : l.Max(x => x.Id) + 1; l.Add(a); Archivo.Guardar(ArchivoNombre, l); return a; }
    }
}
