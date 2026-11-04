using System.Globalization;
using System.Text.Json;
using Ejercicio03_Transporte.Modelos;
using Ejercicio03_Transporte.Servicios;
using Ejercicio03_Transporte.Persistencia;
using Ejercicio05_Artesanias.Modelos;
using Ejercicio05_Artesanias.Servicios;
using Ejercicio05_Artesanias.Persistencia;
using Ejercicio05_Artesanias.Reportes;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
var resultados = new List<(string Nombre, bool Ok, string Detalle)>();
string temporal = Path.Combine(Path.GetTempPath(), "transporte-artesanias-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(temporal);
DateTime ahora = new(2026, 11, 4, 12, 0, 0);
int consecutivo = 0;
void Caso(string nombre, Action prueba)
{
    try
    {
        prueba();
        resultados.Add((nombre, true, "Correcto"));
        Console.WriteLine("OK " + nombre);
    }
    catch (Exception ex)
    {
        resultados.Add((nombre, false, ex.Message));
        Console.WriteLine("ERROR " + nombre + ": " + ex.Message);
    }
}
void Igual<T>(T esperado, T real)
{
    if (!EqualityComparer<T>.Default.Equals(esperado, real)) throw new Exception($"Esperado {esperado}; obtenido {real}");
}
void Rechaza(Action accion)
{
    try { accion(); }
    catch (Exception e) when (e is ArgumentException or InvalidOperationException or IOException or InvalidDataException or JsonException or UnauthorizedAccessException) { return; }
    throw new Exception("La operacion debio rechazarse.");
}
string Capturar(Action accion)
{
    TextWriter salida = Console.Out;
    using var texto = new StringWriter(CultureInfo.InvariantCulture);
    Console.SetOut(texto);
    try { accion(); return texto.ToString(); }
    finally { Console.SetOut(salida); }
}
GestionTransporteService Transporte()
{
    var s = new GestionTransporteService(() => ahora);
    s.RegistrarViaje(new Viaje("V1", "Ibarra", "Quito", ahora.AddHours(5), 10m, TipoViaje.Directo, 4), out _);
    return s;
}
Pasajero Persona(TipoPasajero tipo = TipoPasajero.Normal) => new("1000000001", "Ana Torres", tipo);
string Vender(GestionTransporteService s, int asiento = 1)
{
    Igual(true, s.VenderBoleto("V1", Persona(), asiento, out _));
    return s.Boletos.Last().IdBoleto;
}
(DatosTienda d, JsonRepository r) Tienda()
{
    var r = new JsonRepository(Path.Combine(temporal, "tienda-" + (++consecutivo) + ".json"));
    var d = r.Cargar();
    var c = new Comunidad { Id = "C1", Nombre = "Peguche" };
    d.Comunidades.Add(c);
    d.Comunidades.Add(new Comunidad { Id = "C2", Nombre = "Iluman" });
    d.Emprendedores.Add(new Emprendedor { Id = "E1", ComunidadId = "C1", Nombres = "Ana", Apellidos = "Torres", Cedula = "1" });
    d.Emprendedores.Add(new Emprendedor { Id = "E2", ComunidadId = "C2", Nombres = "Luis", Apellidos = "Mina", Cedula = "2" });
    d.Artesanias.Add(new Artesania { Id = "A1", EmprendedorId = "E1", TipoArtesaniaId = d.TiposArtesanias[0].Id, Nombre = "Poncho", PrecioVenta = 10m });
    d.Artesanias.Add(new Artesania { Id = "A2", EmprendedorId = "E2", TipoArtesaniaId = d.TiposArtesanias[1].Id, Nombre = "Talla", PrecioVenta = 20m });
    var recepcion = new RecepcionService(d, r);
    recepcion.Registrar("E1", new() { new DetalleRecepcion { ArtesaniaId = "A1", CantidadRecibida = 5, PrecioUnitario = 10m } });
    recepcion.Registrar("E2", new() { new DetalleRecepcion { ArtesaniaId = "A2", CantidadRecibida = 4, PrecioUnitario = 20m } });
    return (d, r);
}
string Foto(DatosTienda d) => JsonSerializer.Serialize(d);
try
{
    Caso("03 - Tarifa directa", () => Igual(10m, Transporte().CalcularPrecioBoleto(Transporte().Viajes[0], TipoPasajero.Normal)));
    Caso("03 - Tarifa con paradas 85%", () =>
    {
        var s = Transporte(); s.Viajes[0].Tipo = TipoViaje.ConParadas;
        Igual(8.5m, s.CalcularPrecioBoleto(s.Viajes[0], TipoPasajero.Normal));
    });
    foreach (var tipo in new[] { TipoPasajero.TerceraEdad, TipoPasajero.Discapacidad })
        Caso("03 - Descuento " + tipo, () =>
        {
            var s = Transporte(); Igual(5m, s.CalcularPrecioBoleto(s.Viajes[0], tipo));
            s.Viajes[0].Tipo = TipoViaje.ConParadas; Igual(4.25m, s.CalcularPrecioBoleto(s.Viajes[0], tipo));
        });
    foreach (int capacidad in new[] { 0, -1 })
        Caso("03 - Capacidad invalida " + capacidad, () => Igual(false, Transporte().RegistrarViaje(new Viaje("V2", "Ibarra", "Quito", ahora.AddHours(3), 10, TipoViaje.Directo, capacidad), out _)));
    foreach (decimal tarifa in new[] { 0m, -1m })
        Caso("03 - Tarifa invalida " + tarifa, () => Igual(false, Transporte().RegistrarViaje(new Viaje("V2", "Ibarra", "Quito", ahora.AddHours(3), tarifa, TipoViaje.Directo, 4), out _)));
    Caso("03 - Ruta invalida", () => Igual(false, Transporte().RegistrarViaje(new Viaje("V2", "Ibarra", "Loja", ahora.AddHours(3), 10, TipoViaje.Directo, 4), out _)));
    Caso("03 - Viaje repetido", () => { var s = Transporte(); Igual(false, s.RegistrarViaje(s.Viajes[0], out _)); });
    Caso("03 - Fecha de viaje pasada", () => Igual(false, Transporte().RegistrarViaje(new Viaje("V2", "Ibarra", "Quito", ahora, 10, TipoViaje.Directo, 4), out _)));
    foreach (int asiento in new[] { 0, 5 })
        Caso("03 - Asiento fuera de rango " + asiento, () => Igual(false, Transporte().VenderBoleto("V1", Persona(), asiento, out _)));
    Caso("03 - Asiento ocupado sin cambios", () =>
    {
        var s = Transporte(); Vender(s); Igual(false, s.VenderBoleto("V1", Persona(), 1, out _)); Igual(1, s.Boletos.Count);
    });
    Caso("03 - No duplica asiento si cache fue alterada", () =>
    {
        var s = Transporte(); Vender(s); s.Viajes[0].AsientosOcupados.Clear(); Igual(false, s.VenderBoleto("V1", Persona(), 1, out _));
    });
    Caso("03 - Viaje inexistente", () => Igual(false, Transporte().VenderBoleto("NO", Persona(), 1, out _)));
    Caso("03 - Pasajero invalido", () => Igual(false, Transporte().VenderBoleto("V1", new Pasajero(), 1, out _)));
    Caso("03 - Venta al instante de salida", () => { var s = Transporte(); s.Viajes[0].FechaHoraSalida = ahora; Igual(false, s.VenderBoleto("V1", Persona(), 1, out _)); });
    Caso("03 - Anulacion libera asiento y no se repite", () =>
    {
        var s = Transporte(); var id = Vender(s); Igual(true, s.AnularBoleto(id, out _)); Igual(0, s.Viajes[0].AsientosOcupados.Count); Igual(false, s.AnularBoleto(id, out _)); Vender(s);
    });
    Caso("03 - Limite exacto de dos horas", () => { var s = Transporte(); var id = Vender(s); s.Viajes[0].FechaHoraSalida = ahora.AddHours(2); Igual(true, s.AnularBoleto(id, out _)); });
    Caso("03 - Menos de dos horas", () => { var s = Transporte(); var id = Vender(s); s.Viajes[0].FechaHoraSalida = ahora.AddHours(2).AddTicks(-1); Igual(false, s.AnularBoleto(id, out _)); });
    Caso("03 - Anulacion de viaje inexistente", () => { var s = Transporte(); var id = Vender(s); s.Viajes.Clear(); Igual(false, s.AnularBoleto(id, out _)); Igual(false, s.Boletos[0].EstaAnulado); });
    Caso("03 - No elimina viaje con historial anulado", () => { var s = Transporte(); var id = Vender(s); s.AnularBoleto(id, out _); Igual(false, s.EliminarViaje("V1", out _)); });
    Caso("03 - Elimina viaje sin boletos", () => Igual(true, Transporte().EliminarViaje("V1", out _)));
    Caso("03 - Identificadores unicos", () => { var s = Transporte(); Vender(s, 1); Vender(s, 2); Igual(2, s.Boletos.Select(b => b.IdBoleto).Distinct().Count()); });
    Caso("03 - Auditoria detecta inconsistencia", () =>
    {
        var s = Transporte(); Vender(s); var r = new ReporteService(s);
        Igual(true, Capturar(r.GenerarAuditoriaAsientosLibres).Contains("CORRECTO"));
        s.Viajes[0].AsientosOcupados.Clear(); Igual(true, Capturar(r.GenerarAuditoriaAsientosLibres).Contains("INCONSISTENCIA"));
    });
    Caso("03 - Mapa y porcentaje de ocupacion", () =>
    {
        var s = Transporte(); Vender(s); var r = new ReporteService(s);
        Igual(true, Capturar(() => r.GenerarMapaAsientos("V1")).Contains("[X]"));
        Igual(true, Capturar(r.GenerarPorcentajeOcupacion).Contains("25"));
    });
    Caso("03 - Reportes incluyen dia final y excluyen anulados", () =>
    {
        var s = Transporte(); var id = Vender(s); var r = new ReporteService(s);
        Igual(true, Capturar(() => r.GenerarRecaudacionPorRuta(ahora.Date, ahora.Date)).Contains("10.00"));
        s.AnularBoleto(id, out _);
        Igual(true, Capturar(() => r.GenerarBoletosAnulados(ahora.Date, ahora.Date)).Contains(id));
        Igual(true, Capturar(() => r.GenerarRecaudacionPorRuta(ahora.Date, ahora.Date)).Contains("0.00"));
    });
    Caso("03 - Periodo invertido", () => Rechaza(() => new ReporteService(Transporte()).GenerarRecaudacionPorRuta(ahora.AddDays(1), ahora)));
    Caso("03 - Persistencia entre culturas", () =>
    {
        ManejadorArchivos.RutaCarpeta = Path.Combine(temporal, "transporte"); var s = Transporte(); Vender(s);
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("es-EC"); ManejadorArchivos.GuardarDatos(s);
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US"); var copia = Transporte(); ManejadorArchivos.CargarDatos(copia);
        Igual(10m, copia.Boletos[0].PrecioFinal); Igual(1, copia.Viajes[0].AsientosOcupados.Count);
    });
    Caso("03 - Archivo danado no cambia memoria", () =>
    {
        ManejadorArchivos.RutaCarpeta = Path.Combine(temporal, "corrupto"); Directory.CreateDirectory(ManejadorArchivos.RutaCarpeta);
        File.WriteAllText(Path.Combine(ManejadorArchivos.RutaCarpeta, "transporte.json"), "{"); var s = Transporte();
        Rechaza(() => ManejadorArchivos.CargarDatos(s)); Igual(1, s.Viajes.Count);
    });
    Caso("03 - Guardado rechaza asientos inconsistentes", () => { var s = Transporte(); s.Viajes[0].AsientosOcupados.Add(1); Rechaza(() => ManejadorArchivos.GuardarDatos(s)); });
    Caso("05 - Recepcion e inventario", () => { var (d,r) = Tienda(); Igual(9, new InventarioReporteService(d).GenerarResumen().TotalDisponible); Igual(0, ValidadorTienda.Auditar(d).Count); });
    Caso("05 - Comisiones textiles y madera", () =>
    {
        var (d,r) = Tienda(); new VentaService(d,r).Registrar(new() { ["A1"] = 2, ["A2"] = 1 });
        var textil = new LiquidacionService(d).Generar("E1", DateTime.Today, DateTime.Today);
        Igual(20m, textil.TotalVendido); Igual(4m, textil.TotalComision); Igual(16m, textil.MontoAPagar);
        var madera = new LiquidacionService(d).Generar("E2", DateTime.Today, DateTime.Today);
        Igual(5m, madera.TotalComision); Igual(15m, madera.MontoAPagar);
    });
    foreach (int cantidad in new[] { 0, -1, 6 })
        Caso("05 - Venta invalida " + cantidad, () => { var (d,r) = Tienda(); string antes = Foto(d); Rechaza(() => new VentaService(d,r).Registrar(new() { ["A1"] = cantidad })); Igual(antes, Foto(d)); });
    Caso("05 - Venta completa rechazada sin stock", () => { var (d,r) = Tienda(); string antes = Foto(d); Rechaza(() => new VentaService(d,r).Registrar(new() { ["A1"] = 1, ["A2"] = 5 })); Igual(antes, Foto(d)); });
    Caso("05 - Venta vacia", () => { var (d,r) = Tienda(); Rechaza(() => new VentaService(d,r).Registrar(new())); });
    Caso("05 - Venta inexistente o inactiva", () => { var (d,r) = Tienda(); Rechaza(() => new VentaService(d,r).Registrar(new() { ["NO"] = 1 })); d.Artesanias[0].Activa = false; Rechaza(() => new VentaService(d,r).Registrar(new() { ["A1"] = 1 })); });
    Caso("05 - FIFO entre dos recepciones", () =>
    {
        var (d,r) = Tienda(); d.Recepciones[0].Fecha = DateTime.Now.AddDays(-1);
        new RecepcionService(d,r).Registrar("E1", new() { new DetalleRecepcion { ArtesaniaId = "A1", CantidadRecibida = 3, PrecioUnitario = 10 } });
        new VentaService(d,r).Registrar(new() { ["A1"] = 6 });
        Igual(5, d.Ventas[0].Detalles[0].CantidadVendida); Igual(1, d.Ventas[0].Detalles[1].CantidadVendida);
    });
    Caso("05 - Devolucion al propietario", () =>
    {
        var (d,r) = Tienda(); string lote = d.Recepciones[0].Detalles[0].Id;
        new DevolucionService(d,r).Registrar("E1", new() { [lote] = 2 });
        Igual(3, new InventarioService(d).ObtenerCantidadDisponible(lote));
    });
    Caso("05 - Devolucion a otro propietario rechazada", () => { var (d,r) = Tienda(); Rechaza(() => new DevolucionService(d,r).Registrar("E2", new() { [d.Recepciones[0].Detalles[0].Id] = 1 })); Igual(0, d.Devoluciones.Count); });
    foreach (int cantidad in new[] { 0, -1, 6 })
        Caso("05 - Devolucion invalida " + cantidad, () => { var (d,r) = Tienda(); Rechaza(() => new DevolucionService(d,r).Registrar("E1", new() { [d.Recepciones[0].Detalles[0].Id] = cantidad })); Igual(0, d.Devoluciones.Count); });
    Caso("05 - Recepcion de otro propietario", () => { var (d,r) = Tienda(); Rechaza(() => new RecepcionService(d,r).Registrar("E2", new() { new DetalleRecepcion { ArtesaniaId = "A1", CantidadRecibida = 1, PrecioUnitario = 10 } })); });
    Caso("05 - Recepcion repetida o cantidad invalida", () => { var (d,r) = Tienda(); var l = new DetalleRecepcion { ArtesaniaId = "A1", CantidadRecibida = 1, PrecioUnitario = 10 }; Rechaza(() => new RecepcionService(d,r).Registrar("E1", new() { l, l })); l.CantidadRecibida = 0; Rechaza(() => new RecepcionService(d,r).Registrar("E1", new() { l })); });
    Caso("05 - No elimina emprendedor con artesanias", () => { var (d,r) = Tienda(); Rechaza(() => new EmprendedorService(d,r).Desactivar("E1")); Igual(true, d.Emprendedores[0].Activo); });
    Caso("05 - No elimina artesania con movimientos", () => { var (d,r) = Tienda(); Rechaza(() => new ArtesaniaService(d,r).Desactivar("A1")); Igual(true, d.Artesanias[0].Activa); });
    Caso("05 - No cambia propietario con historial", () => { var (d,r) = Tienda(); Rechaza(() => new ArtesaniaService(d,r).Actualizar("A1", "E2", d.TiposArtesanias[0].Id, "Poncho", "", 10)); Igual("E1", d.Artesanias[0].EmprendedorId); });
    Caso("05 - Limite de noventa dias en ambos reportes", () =>
    {
        var (d,r) = Tienda(); DateTime corte = DateTime.Now; d.Recepciones[0].Fecha = corte.AddDays(-90);
        Igual(0, new DevolucionReporteService(d).Generar(corte).Count); Igual(0, new InventarioService(d).ObtenerArtesaniasParaDevolver(corte).Count);
        Igual(1, new DevolucionReporteService(d).Generar(corte.AddSeconds(1)).Count); Igual(1, new InventarioService(d).ObtenerArtesaniasParaDevolver(corte.AddSeconds(1)).Count);
    });
    Caso("05 - Ranking de comunidades y artesanias", () =>
    {
        var (d,r) = Tienda(); new VentaService(d,r).Registrar(new() { ["A1"] = 3, ["A2"] = 1 });
        var reportes = new VentasReporteService(d);
        Igual("C1", reportes.GenerarPorComunidad(DateTime.Today, DateTime.Today)[0].ComunidadId);
        Igual("A1", reportes.GenerarArtesaniasMasVendidas(DateTime.Today, DateTime.Today)[0].ArtesaniaId);
    });
    Caso("05 - Periodos de movimientos incluyen dia final", () =>
    {
        var (d,r) = Tienda(); var s = new VentaService(d,r); s.Registrar(new() { ["A1"] = 1 }); Igual(1, s.ListarPorPeriodo(DateTime.Today, DateTime.Today).Count);
        var dev = new DevolucionService(d,r); dev.Registrar("E1", new() { [d.Recepciones[0].Detalles[0].Id] = 1 }); Igual(1, dev.ListarPorPeriodo(DateTime.Today, DateTime.Today).Count);
    });
    Caso("05 - Periodos invertidos", () => { var (d,r) = Tienda(); Rechaza(() => new LiquidacionService(d).GenerarTodas(DateTime.Today.AddDays(1), DateTime.Today)); Rechaza(() => new VentasReporteService(d).GenerarPorComunidad(DateTime.Today.AddDays(1), DateTime.Today)); });
    Caso("05 - Auditoria detecta stock negativo", () => { var (d,r) = Tienda(); d.Recepciones[0].Detalles[0].CantidadRecibida = -1; Igual(true, ValidadorTienda.Auditar(d).Count > 0); });
    Caso("05 - Auditoria detecta referencias huerfanas", () => { var (d,r) = Tienda(); d.Ventas.Add(new Venta { Detalles = new() { new DetalleVenta { DetalleRecepcionId = "NO", CantidadVendida = 1, PrecioUnitario = 10, PorcentajeComision = 20 } } }); Igual(true, ValidadorTienda.Auditar(d).Count > 0); });
    Caso("05 - Comisiones invalidas no se guardan", () => { var (d,r) = Tienda(); d.TiposArtesanias[0].PorcentajeComision = 30; Rechaza(() => r.Guardar(d)); });
    foreach (string operacion in new[] { "venta", "recepcion", "devolucion", "actualizacion" })
        Caso("05 - Fallo de guardado revierte " + operacion, () =>
        {
            var (d,r) = Tienda(); string bloqueo = Path.Combine(temporal, "bloqueo-" + Guid.NewGuid()); File.WriteAllText(bloqueo, "ocupado");
            var malo = new JsonRepository(Path.Combine(bloqueo, "tienda.json")); string antes = Foto(d);
            Rechaza(() =>
            {
                if (operacion == "venta") new VentaService(d,malo).Registrar(new() { ["A1"] = 1 });
                if (operacion == "recepcion") new RecepcionService(d,malo).Registrar("E1", new() { new DetalleRecepcion { ArtesaniaId = "A1", CantidadRecibida = 1, PrecioUnitario = 10 } });
                if (operacion == "devolucion") new DevolucionService(d,malo).Registrar("E1", new() { [d.Recepciones[0].Detalles[0].Id] = 1 });
                if (operacion == "actualizacion") new ArtesaniaService(d,malo).Actualizar("A1", "E1", d.TiposArtesanias[0].Id, "Otro", "", 15);
            });
            Igual(antes, Foto(d));
        });
    Caso("05 - Persistencia conserva movimientos y saldos", () => { var (d,r) = Tienda(); new VentaService(d,r).Registrar(new() { ["A1"] = 1 }); Igual(Foto(d), Foto(r.Cargar())); });
    Caso("05 - JSON danado no se reemplaza", () => { string p = Path.Combine(temporal, "danado.json"); File.WriteAllText(p, "{"); Rechaza(() => new JsonRepository(p).Cargar()); Igual("{", File.ReadAllText(p)); });
    Caso("03 - Importa los TXT de ejemplo completos", () =>
    {
        string raiz = Path.GetFullPath("../../../../../", AppContext.BaseDirectory);
        ManejadorArchivos.RutaCarpeta = Path.Combine(raiz, "datos-prueba/Ejercicio03_Transporte/datos");
        var s = Transporte(); ManejadorArchivos.CargarDatos(s); Igual(4, s.Viajes.Count); Igual(3, s.Boletos.Count);
    });
    Caso("05 - Auditoria rechaza identificadores repetidos", () =>
    {
        var (d,r) = Tienda(); d.Artesanias[1].Id = d.Artesanias[0].Id;
        Igual(true, ValidadorTienda.Auditar(d).Count > 0); Rechaza(() => r.Guardar(d));
    });
    Caso("05 - Historico conserva precio y comision", () =>
    {
        var (d,r) = Tienda(); new VentaService(d,r).Registrar(new() { ["A1"] = 1 });
        new ArtesaniaService(d,r).Actualizar("A1", "E1", d.TiposArtesanias[1].Id, "Poncho", "", 99);
        Igual(10m, d.Ventas[0].Detalles[0].TotalVendido); Igual(2m, d.Ventas[0].Detalles[0].ComisionTienda);
    });
    foreach (string ejercicio in new[] { "Ejercicio03_Transporte", "Ejercicio05_Artesanias" })
    {
        foreach (string entrada in new[] { "salir", "eof", "invalida", "reportes" })
        {
            Caso(ejercicio + " - Menu " + entrada, () =>
            {
                string raiz = Path.GetFullPath("../../../../../", AppContext.BaseDirectory);
                string dll = Path.Combine(raiz, "src", ejercicio, "bin/Debug/net10.0", ejercicio + ".dll");
                var info = new System.Diagnostics.ProcessStartInfo("dotnet")
                {
                    RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true,
                    CreateNoWindow = true, WorkingDirectory = temporal
                };
                info.ArgumentList.Add(dll);
                string ruta = Path.Combine(temporal, "menu-" + (++consecutivo));
                info.ArgumentList.Add(ejercicio.Contains("03") ? ruta : ruta + ".json");
                using var proceso = System.Diagnostics.Process.Start(info)!;
                string salir = ejercicio.Contains("03") ? "8" : "0";
                string entradas = entrada switch
                {
                    "eof" => "",
                    "invalida" => "abc\n" + salir + "\n",
                    "reportes" when ejercicio.Contains("03") => "1\n6\n1\nV01\n6\n2\n2026-01-01\n2027-01-01\n6\n3\n6\n4\n2026-01-01\n2027-01-01\n6\n5\n7\n8\n",
                    "reportes" => "7\n\n9\n\n11\n\n0\n",
                    _ => salir + "\n"
                };
                proceso.StandardInput.Write(entradas); proceso.StandardInput.Close();
                var salida = proceso.StandardOutput.ReadToEndAsync(); var errores = proceso.StandardError.ReadToEndAsync();
                if (!proceso.WaitForExit(10000)) { proceso.Kill(true); throw new Exception("El menu no termina."); }
                string texto = salida.GetAwaiter().GetResult();
                if (proceso.ExitCode != 0) throw new Exception(errores.GetAwaiter().GetResult());
                Igual(true, texto.Contains(ejercicio.Contains("03") ? "TRANSPORTE" : "ARTESANIAS"));
            });
        }
    }
    if (args.Length > 1)
    {
        var (d,r) = Tienda();
        d.Recepciones[0].Fecha = DateTime.Now.AddDays(-91);
        new VentaService(d,r).Registrar(new() { ["A1"] = 2, ["A2"] = 1 });
        new DevolucionService(d,r).Registrar("E1", new() { [d.Recepciones[0].Detalles[0].Id] = 1 });
        new JsonRepository(args[1]).Guardar(d);
    }
    Caso("05 - Redondeo monetario", () => Igual(0.03m, new DetalleVenta { CantidadVendida = 1, PrecioUnitario = 0.15m, PorcentajeComision = 20 }.ComisionTienda));
}
finally
{
    Directory.Delete(temporal, true);
}
int fallos = resultados.Count(r => !r.Ok);
string informe = "# Pruebas de los ejercicios 3 y 5\n\nCasos ejecutados: " + resultados.Count + ". Correctos: " + (resultados.Count - fallos) + ". Fallidos: " + fallos + ".\n\n| Caso | Resultado | Detalle |\n| --- | --- | --- |\n";
informe += string.Join("\n", resultados.Select(r => $"| {r.Nombre} | {(r.Ok ? "OK" : "ERROR")} | {r.Detalle.Replace("|", "/")} |"));
if (args.Length > 0) File.WriteAllText(args[0], informe + "\n");
Console.WriteLine($"{resultados.Count - fallos}/{resultados.Count} pruebas correctas.");
return fallos == 0 ? 0 : 1;
