using Ejercicio04_Cafeteria.Modelos;
using Ejercicio04_Cafeteria.Persistencia;

namespace Ejercicio04_Cafeteria.Servicios
{
    public class InventarioService
    {
        private const string ArchivoNombre = "movimientos.json";
        public List<Movimiento> Movimientos() => Archivo.Leer<Movimiento>(ArchivoNombre);
        public void Entrada(Ingrediente ingrediente, decimal cantidad, string descripcion = "Abastecimiento")
        {
            if (cantidad <= 0) throw new ArgumentException("La cantidad debe ser positiva.");
            Registrar(ingrediente, cantidad, "Entrada", descripcion);
        }
        public void Salida(Ingrediente ingrediente, decimal cantidad, string descripcion = "Venta")
        {
            if (cantidad <= 0 || ingrediente.Stock < cantidad) throw new InvalidOperationException("Stock insuficiente.");
            Registrar(ingrediente, -cantidad, "Salida", descripcion);
        }
        private static void Registrar(Ingrediente ingrediente, decimal cambio, string tipo, string descripcion)
        {
            var anterior = ingrediente.Stock; ingrediente.Stock += cambio;
            var l = Archivo.Leer<Movimiento>(ArchivoNombre);
            l.Add(new Movimiento { Id = l.Count == 0 ? 1 : l.Max(x => x.Id) + 1, IngredienteId = ingrediente.Id, Tipo = tipo, Cantidad = Math.Abs(cambio), StockAnterior = anterior, StockNuevo = ingrediente.Stock, Fecha = DateTime.Now, Descripcion = descripcion });
            Archivo.Guardar(ArchivoNombre, l);
        }
    }
}
