using Ejercicio04_Cafeteria.Modelos;
using Ejercicio04_Cafeteria.Persistencia;
using Ejercicio04_Cafeteria.Reportes;
using Ejercicio04_Cafeteria.Servicios;

var correctas = 0;
var pruebas = new List<(string Nombre, Action Ejecutar)>
{
    ("Precio del plato e IVA de bebida", () =>
    {
        Reiniciar();
        Comprobar(new PlatoService().CalcularPrecio(1) == 4m, "Precio del plato incorrecto");
        Comprobar(new BebidaService().PrecioFinal(1) == 2.30m, "IVA incorrecto");
    }),
    ("Venta valida descuenta y registra movimientos", () =>
    {
        Reiniciar();
        var venta = new VentaService().Registrar([new() { ProductoId = 1, Tipo = "Plato", Cantidad = 2 }, new() { ProductoId = 1, Tipo = "Bebida", Cantidad = 1 }]);
        Comprobar(venta.Total == 10.30m, "Total incorrecto");
        Comprobar(Archivo.Leer<Ingrediente>("ingredientes.json")[0].Stock == 3m, "No descontó ingredientes");
        Comprobar(Archivo.Leer<Bebida>("bebidas.json")[0].Stock == 4, "No descontó bebida");
        Comprobar(Archivo.Leer<Movimiento>("movimientos.json").Count == 1, "Falta movimiento");
    }),
    ("Bebida repetida no deja stock negativo", () =>
    {
        Reiniciar();
        Rechazar(() => new VentaService().Registrar([new() { ProductoId = 1, Tipo = "Bebida", Cantidad = 4 }, new() { ProductoId = 1, Tipo = "Bebida", Cantidad = 4 }]));
        Comprobar(Archivo.Leer<Bebida>("bebidas.json")[0].Stock == 5 && new VentaService().Listar().Count == 0, "La venta modificó los datos");
    }),
    ("Ingredientes insuficientes rechazan toda la venta", () =>
    {
        Reiniciar();
        Rechazar(() => new VentaService().Registrar([new() { ProductoId = 1, Tipo = "Bebida", Cantidad = 1 }, new() { ProductoId = 1, Tipo = "Plato", Cantidad = 11 }]));
        Comprobar(Archivo.Leer<Bebida>("bebidas.json")[0].Stock == 5 && Archivo.Leer<Ingrediente>("ingredientes.json")[0].Stock == 5m && new VentaService().Listar().Count == 0, "Hubo venta parcial");
    }),
    ("Tipo de producto invalido", () =>
    {
        Reiniciar();
        Rechazar(() => new VentaService().Registrar([new() { ProductoId = 1, Tipo = "Otro", Cantidad = 1 }]));
        Comprobar(new VentaService().Listar().Count == 0, "Se guardó venta inválida");
    }),
    ("Ultimo dia incluido en reporte", () =>
    {
        Reiniciar();
        Archivo.Guardar("ventas.json", new List<Venta> { new() { Id = 1, Fecha = new DateTime(2026, 1, 10, 17, 30, 0), Detalles = [new() { ProductoId = 1, Tipo = "Plato", Cantidad = 2, Precio = 4m }], Total = 8m } });
        var filas = new ReporteVentas().Obtener(new DateTime(2026, 1, 10), new DateTime(2026, 1, 10)).ToList();
        Comprobar(filas.Count == 1 && filas[0].Cantidad == 2 && filas[0].Total == 8m, "Falta venta del último día");
        Rechazar(() => new ReporteVentas().Obtener(new DateTime(2026, 1, 11), new DateTime(2026, 1, 10)).ToList());
    }),
    ("Preparacion y lista de compras", () =>
    {
        Reiniciar();
        Comprobar(new ReportePreparacion().Obtener()["Torta"] == 5, "Cantidad preparable incorrecta");
        Comprobar(new ReporteCompras().Obtener().Single().Nombre == "Harina", "Lista de compras incorrecta");
    }),
    ("Ingrediente en receta no se elimina", () =>
    {
        Reiniciar();
        Rechazar(() => new IngredienteService(new PlatoService()).Eliminar(1));
        Comprobar(Archivo.Leer<Ingrediente>("ingredientes.json").Count == 1, "Se eliminó ingrediente usado");
    })
};

foreach (var (nombre, ejecutar) in pruebas)
{
    try { ejecutar(); correctas++; Console.WriteLine($"OK {nombre}"); }
    catch (Exception e) { Console.WriteLine($"ERROR {nombre}: {e.Message}"); }
}
Console.WriteLine($"{correctas}/{pruebas.Count} pruebas correctas.");
if (correctas != pruebas.Count) Environment.ExitCode = 1;

static void Reiniciar()
{
    Archivo.Guardar("ingredientes.json", new List<Ingrediente> { new() { Id = 1, Nombre = "Harina", Unidad = "kg", Stock = 5m, StockMinimo = 6m, Costo = 2m } });
    Archivo.Guardar("bebidas.json", new List<Bebida> { new() { Id = 1, Nombre = "Agua", Stock = 5, Precio = 2m } });
    Archivo.Guardar("platos.json", new List<Plato> { new() { Id = 1, Nombre = "Torta" } });
    Archivo.Guardar("recetas.json", new List<Receta> { new() { Id = 1, PlatoId = 1, Ingredientes = [new() { IngredienteId = 1, Cantidad = 1m }] } });
    Archivo.Guardar("ventas.json", new List<Venta>());
    Archivo.Guardar("movimientos.json", new List<Movimiento>());
}
static void Comprobar(bool condicion, string mensaje) { if (!condicion) throw new Exception(mensaje); }
static void Rechazar(Action accion)
{
    try { accion(); }
    catch (ArgumentException) { return; }
    catch (InvalidOperationException) { return; }
    throw new Exception("La operación debió rechazarse");
}
