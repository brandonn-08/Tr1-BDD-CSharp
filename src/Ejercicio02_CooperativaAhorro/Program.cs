using System.Globalization;
using Ejercicio02_CooperativaAhorro.Servicios;
using Ejercicio02_CooperativaAhorro.Reportes;

// Se puede indicar un archivo distinto para no modificar los datos de ejemplo.
string ruta = args.Length > 0 ? args[0] : Path.Combine(AppContext.BaseDirectory, "datos", "cooperativa.json");
Cooperativa cooperativa;
try { cooperativa = new Cooperativa(ruta); }
catch (Exception e) when (e is IOException or UnauthorizedAccessException or System.Text.Json.JsonException or InvalidDataException)
{ Console.WriteLine("No se pudo cargar el archivo: " + e.Message); return 1; }
Console.WriteLine("COOPERATIVA DE AHORRO Y CREDITO - IMBABURA");
Console.WriteLine("Archivo: " + Path.GetFullPath(ruta));
while (true)
{
    Console.WriteLine("\n1. Registrar socio  2. Abrir cuenta  3. Listar socios y cuentas");
    Console.WriteLine("4. Depositar  5. Retirar  6. Transferir  7. Aplicar intereses del mes");
    Console.WriteLine("8. Cerrar cuenta  9. Reportes  0. Salir");
    Console.Write("Opcion: ");
    string? opcion = Console.ReadLine();
    if (opcion is null or "0") break;
    try
    {
        switch (opcion)
        {
            case "1": cooperativa.AgregarSocio(Entero("Id del socio"), Texto("Nombre")); break;
            case "2":
                int numero = Entero("Numero de cuenta"), socio = Entero("Id del socio");
                string tipo = Texto("Tipo: A = ahorro, C = corriente").ToUpperInvariant();
                if (tipo != "A" && tipo != "C") throw new InvalidOperationException("Tipo no valido.");
                cooperativa.AbrirCuenta(numero, socio, tipo == "A", Fecha("Fecha de apertura"));
                Console.WriteLine("Cuenta abierta en cero. En ahorro el primer deposito debe ser al menos $5."); break;
            case "3":
                var datos = cooperativa.Consultar();
                foreach (var s in datos.Socios) Console.WriteLine($"Socio {s.Id}: {s.Nombre}");
                foreach (var c in datos.Cuentas) Console.WriteLine($"Cuenta {c.Numero} | {c.GetType().Name} | Socio {c.SocioId} | Saldo {c.Saldo:F2} | Activa: {c.Activa}");
                break;
            case "4": cooperativa.Depositar(Entero("Cuenta"), Dinero(), Fecha("Fecha")); break;
            case "5": cooperativa.Retirar(Entero("Cuenta"), Dinero(), Fecha("Fecha")); break;
            case "6": cooperativa.Transferir(Entero("Socio"), Entero("Cuenta origen"), Entero("Cuenta destino"), Dinero(), Fecha("Fecha")); break;
            case "7": cooperativa.AplicarIntereses(Entero("Anio"), Entero("Mes")); break;
            case "8": cooperativa.CerrarCuenta(Entero("Cuenta")); break;
            case "9": MostrarReportes(); break;
            default: Console.WriteLine("Opcion no valida."); continue;
        }
        Console.WriteLine("Operacion terminada.");
    }
    catch (Exception e) when (e is InvalidOperationException or FormatException or OverflowException or ArgumentOutOfRangeException or IOException or UnauthorizedAccessException)
    { Console.WriteLine("No se realizo la operacion: " + e.Message); }
}
return 0;

string Texto(string mensaje)
{
    Console.Write(mensaje + ": ");
    return Console.ReadLine() ?? throw new InvalidOperationException("Entrada finalizada.");
}
int Entero(string mensaje) => int.Parse(Texto(mensaje), CultureInfo.InvariantCulture);
DateOnly Fecha(string mensaje) => DateOnly.ParseExact(Texto(mensaje + " (aaaa-mm-dd)"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
decimal Dinero() => decimal.Parse(Texto("Monto (ej. 10.50)").Replace(',', '.'), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
void MostrarReportes()
{
    Console.WriteLine("1. Estado de cuenta  2. Intereses del mes  3. Sobregiros  4. Totales del periodo  5. Auditoria");
    string reporte = Texto("Reporte"); var d = cooperativa.Consultar();
    switch (reporte)
    {
        case "1":
            int n = Entero("Cuenta");
            Console.WriteLine("ID | Fecha | Tipo | Monto | Saldo posterior | Detalle");
            foreach (var m in ReportesCooperativa.EstadoCuenta(d, n))
                Console.WriteLine($"{m.Id} | {m.Fecha:yyyy-MM-dd} | {m.Tipo} | {m.Monto:F2} | {m.SaldoPosterior:F2} | {m.Detalle}");
            Console.WriteLine($"Saldo actual: {d.Cuentas.Single(c => c.Numero == n).Saldo:F2}"); break;
        case "2":
            foreach (var i in ReportesCooperativa.InteresesMes(d, Entero("Anio"), Entero("Mes")))
                Console.WriteLine($"Cuenta {i.Cuenta} | Base {i.Saldo:F2} | Calculado {i.Calculado:F2} | Acreditado {i.Acreditado:F2} | Aplicado: {i.Aplicado}"); break;
        case "3":
            foreach (var c in ReportesCooperativa.Sobregiros(d)) Console.WriteLine($"Cuenta {c.Numero} | Socio {c.SocioId} | Saldo {c.Saldo:F2}"); break;
        case "4":
            var t = ReportesCooperativa.Totales(d, Fecha("Desde"), Fecha("Hasta"));
            Console.WriteLine($"Depositado: {t.Depositado:F2} | Retirado: {t.Retirado:F2}"); break;
        case "5":
            foreach (var a in ReportesCooperativa.Auditar(d)) Console.WriteLine($"Cuenta {a.Cuenta} | Guardado {a.Guardado:F2} | Calculado {a.Calculado:F2} | Correcta: {a.Correcta}"); break;
        default: throw new InvalidOperationException("Reporte no valido.");
    }
}
