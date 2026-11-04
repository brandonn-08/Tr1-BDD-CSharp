using Ejercicio04_Cafeteria.Modelos;
using Ejercicio04_Cafeteria.Persistencia;

namespace Ejercicio04_Cafeteria.Servicios
{
    public class VentaService
    {
        public List<Venta> Listar() => Archivo.Leer<Venta>("ventas.json");
        public Venta Registrar(List<DetalleVenta> detalles)
        {
            if (detalles is null || detalles.Count == 0 || detalles.Any(x => x.Cantidad <= 0)) throw new ArgumentException("La venta no contiene detalles válidos.");
            var ingredientes = Archivo.Leer<Ingrediente>("ingredientes.json");
            var bebidas = Archivo.Leer<Bebida>("bebidas.json");
            var recetas = Archivo.Leer<Receta>("recetas.json");
            var platos = Archivo.Leer<Plato>("platos.json");
            var requeridos = new Dictionary<int, decimal>();
            var bebidasRequeridas = new Dictionary<int, int>();
            foreach (var d in detalles)
            {
                if (d.Tipo is null || (!d.Tipo.Equals("Bebida", StringComparison.OrdinalIgnoreCase) && !d.Tipo.Equals("Plato", StringComparison.OrdinalIgnoreCase)))
                    throw new ArgumentException("El tipo debe ser Plato o Bebida.");
                if (d.Tipo.Equals("Bebida", StringComparison.OrdinalIgnoreCase))
                {
                    var b = bebidas.FirstOrDefault(x => x.Id == d.ProductoId) ?? throw new KeyNotFoundException("Bebida inexistente.");
                    bebidasRequeridas[b.Id] = bebidasRequeridas.GetValueOrDefault(b.Id) + d.Cantidad;
                    d.Nombre = b.Nombre; d.Precio = b.Precio * 1.15m;
                }
                else
                {
                    var p = platos.FirstOrDefault(x => x.Id == d.ProductoId) ?? throw new KeyNotFoundException("Plato inexistente.");
                    var r = recetas.FirstOrDefault(x => x.PlatoId == p.Id) ?? throw new InvalidOperationException("El plato no tiene receta.");
                    foreach (var item in r.Ingredientes) requeridos[item.IngredienteId] = requeridos.GetValueOrDefault(item.IngredienteId) + item.Cantidad * d.Cantidad;
                    d.Nombre = p.Nombre; d.Precio = r.Ingredientes.Sum(x => x.Cantidad * (ingredientes.First(i => i.Id == x.IngredienteId).Costo)) * 2m;
                }
            }
            if (bebidasRequeridas.Any(x => bebidas.First(b => b.Id == x.Key).Stock < x.Value)) throw new InvalidOperationException("Stock insuficiente de bebida.");
            if (requeridos.Any(x => ingredientes.FirstOrDefault(i => i.Id == x.Key) is not { } ingrediente || ingrediente.Stock < x.Value)) throw new InvalidOperationException("Stock insuficiente de ingredientes.");
            var copiaIngredientes = ingredientes.Select(Copiar).ToList(); var copiaBebidas = bebidas.Select(Copiar).ToList(); var copiaVentas = Listar(); var copiaMov = Archivo.Leer<Movimiento>("movimientos.json");
            try
            {
                var movimientos = copiaMov.ToList();
                var siguienteId = movimientos.Count == 0 ? 1 : movimientos.Max(x => x.Id) + 1;
                foreach (var req in requeridos) { var i = ingredientes.First(x => x.Id == req.Key); var anterior = i.Stock; i.Stock -= req.Value; movimientos.Add(new Movimiento { Id = siguienteId++, IngredienteId = i.Id, Tipo = "Salida", Cantidad = req.Value, StockAnterior = anterior, StockNuevo = i.Stock, Fecha = DateTime.Now, Descripcion = "Venta" }); }
                foreach (var req in bebidasRequeridas) bebidas.First(x => x.Id == req.Key).Stock -= req.Value;
                var venta = new Venta { Id = copiaVentas.Count == 0 ? 1 : copiaVentas.Max(x => x.Id) + 1, Fecha = DateTime.Now, Detalles = detalles, Total = detalles.Sum(x => x.Cantidad * x.Precio) };
                Archivo.Guardar("ingredientes.json", ingredientes); Archivo.Guardar("bebidas.json", bebidas); Archivo.Guardar("movimientos.json", movimientos); copiaVentas.Add(venta); Archivo.Guardar("ventas.json", copiaVentas); return venta;
            }
            catch
            {
                Archivo.Guardar("ingredientes.json", copiaIngredientes); Archivo.Guardar("bebidas.json", copiaBebidas); Archivo.Guardar("ventas.json", copiaVentas); Archivo.Guardar("movimientos.json", copiaMov); throw;
            }
        }
        private static Ingrediente Copiar(Ingrediente x) => new() { Id = x.Id, Nombre = x.Nombre, Unidad = x.Unidad, Stock = x.Stock, StockMinimo = x.StockMinimo, Costo = x.Costo };
        private static Bebida Copiar(Bebida x) => new() { Id = x.Id, Nombre = x.Nombre, Precio = x.Precio, Stock = x.Stock };
    }
}
