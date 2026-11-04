using Ejercicio04_Cafeteria.Modelos;
using Ejercicio04_Cafeteria.Reportes;
using Ejercicio04_Cafeteria.Servicios;

var platos = new PlatoService();
var ingredientes = new IngredienteService(platos);
var bebidas = new BebidaService();
var inventario = new InventarioService();
var abastecimientos = new AbastecimientoService(inventario);
var ventas = new VentaService();
var salir = false;

while (!salir)
{
    Console.WriteLine("\nCAFETERÍA UTN\n1. Ingredientes\n2. Platos y recetas\n3. Bebidas\n4. Abastecimiento\n5. Venta\n6. Inventario\n7. Reportes\n0. Salir");
    Console.Write("Seleccione una opción: ");
    var entrada = Console.ReadLine();

    if (entrada is null)
    {
        Console.WriteLine("No se recibió ninguna entrada. El programa finalizará.");
        break;
    }

    if (!int.TryParse(entrada, out var opcion))
    {
        Console.WriteLine("Opción no válida. Introduzca un número del 0 al 7.");
        continue;
    }

    switch (opcion)
    {
        case 1: MenuIngredientes(ingredientes); break;
        case 2: MenuPlatos(platos); break;
        case 3: MenuBebidas(bebidas); break;
        case 4: RegistrarAbastecimiento(ingredientes, bebidas, abastecimientos); break;
        case 5: RegistrarVenta(ventas); break;
        case 6: new ReporteInventario().Mostrar(); break;
        case 7: MenuReportes(platos); break;
        case 0: salir = true; break;
        default: Console.WriteLine("Opción no válida."); break;
    }
}

static void MenuIngredientes(IngredienteService s)
{
    Console.WriteLine("1. Listar  2. Registrar  3. Eliminar");
    switch (Console.ReadLine())
    {
        case "1": foreach (var x in s.Listar()) Console.WriteLine($"{x.Id}: {x.Nombre}, {x.Stock:F2} {x.Unidad}"); break;
        case "2": try { s.Registrar(new Ingrediente { Nombre = LeerTexto("Nombre"), Unidad = LeerTexto("Unidad (kg/litros)"), Stock = LeerDecimal("Stock"), StockMinimo = LeerDecimal("Stock mínimo"), Costo = LeerDecimal("Costo") }); Console.WriteLine("Ingrediente registrado."); } catch (Exception e) { Console.WriteLine(e.Message); } break;
        case "3": try { s.Eliminar(LeerEntero("Id")); Console.WriteLine("Ingrediente eliminado."); } catch (Exception e) { Console.WriteLine(e.Message); } break;
    }
}

static void MenuPlatos(PlatoService s)
{
    Console.WriteLine("1. Listar  2. Registrar  3. Asignar receta");
    var opcion = Console.ReadLine();
    try { if (opcion == "1") foreach (var x in s.Listar()) Console.WriteLine($"{x.Id}: {x.Nombre}"); else if (opcion == "2") s.Registrar(new Plato { Nombre = LeerTexto("Nombre") }); else if (opcion == "3") { var r = new Receta { PlatoId = LeerEntero("Id del plato") }; for (var i = 0; i < LeerEntero("Cantidad de ingredientes"); i++) r.Ingredientes.Add(new DetalleReceta { IngredienteId = LeerEntero("Id ingrediente"), Cantidad = LeerDecimal("Cantidad") }); s.AsignarReceta(r); Console.WriteLine("Receta guardada."); } } catch (Exception e) { Console.WriteLine(e.Message); }
}

static void MenuBebidas(BebidaService s)
{
    Console.WriteLine("1. Listar  2. Registrar");
    if (Console.ReadLine() == "1") foreach (var x in s.Listar()) Console.WriteLine($"{x.Id}: {x.Nombre}, {x.Precio * 1.15m:F2} final, stock {x.Stock}");
    else try { s.Registrar(new Bebida { Nombre = LeerTexto("Nombre"), Precio = LeerDecimal("Precio base"), Stock = LeerEntero("Stock") }); Console.WriteLine("Bebida registrada."); } catch (Exception e) { Console.WriteLine(e.Message); }
}

static void RegistrarAbastecimiento(IngredienteService si, BebidaService sb, AbastecimientoService sa)
{
    try { var tipo = LeerTexto("Tipo (Ingrediente/Bebida)"); if (tipo.Equals("Ingrediente", StringComparison.OrdinalIgnoreCase)) { var i = si.Buscar(LeerEntero("Id")) ?? throw new KeyNotFoundException("No existe."); sa.RegistrarIngrediente(i, LeerDecimal("Cantidad"), LeerDecimal("Costo de compra")); si.Actualizar(i); } else { var b = sb.Buscar(LeerEntero("Id")) ?? throw new KeyNotFoundException("No existe."); sa.RegistrarBebida(b, LeerEntero("Cantidad"), LeerDecimal("Costo")); sb.Actualizar(b); } Console.WriteLine("Abastecimiento registrado."); } catch (Exception e) { Console.WriteLine(e.Message); }
}

static void RegistrarVenta(VentaService s)
{
    try { var detalles = new List<DetalleVenta>(); for (var i = 0; i < LeerEntero("Cantidad de productos"); i++) detalles.Add(new DetalleVenta { ProductoId = LeerEntero("Id producto"), Tipo = LeerTexto("Tipo (Plato/Bebida)"), Cantidad = LeerEntero("Cantidad") }); var v = s.Registrar(detalles); Console.WriteLine($"Venta {v.Id} registrada. Total: {v.Total:F2}"); } catch (Exception e) { Console.WriteLine(e.Message); }
}

static void MenuReportes(PlatoService platos)
{
    Console.WriteLine("1. Platos  2. Compras  3. Preparación  4. Ventas");
    switch (Console.ReadLine()) { case "1": new ReportePlatos().Mostrar(platos); break; case "2": new ReporteCompras().Mostrar(); break; case "3": new ReportePreparacion().Mostrar(); break; case "4": new ReporteVentas().Mostrar(LeerFecha("Inicio"), LeerFecha("Fin")); break; }
}

static string LeerTexto(string mensaje) { Console.Write($"{mensaje}: "); return Console.ReadLine()?.Trim() ?? ""; }
static int LeerEntero(string mensaje) { while (true) if (int.TryParse(LeerTexto(mensaje), out var x) && x >= 0) return x; else Console.WriteLine("Ingrese un entero válido."); }
static decimal LeerDecimal(string mensaje) { while (true) if (decimal.TryParse(LeerTexto(mensaje), out var x) && x >= 0) return x; else Console.WriteLine("Ingrese un número válido."); }
static DateTime LeerFecha(string mensaje) { while (true) if (DateTime.TryParse(LeerTexto(mensaje), out var x)) return x; else Console.WriteLine("Ingrese una fecha válida."); }