using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Biblioteca.Modelos;

string raiz = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
string programa = Path.Combine(raiz, "src", "Ejercicio01_Biblioteca", "bin", "Debug", "net10.0", "Ejercicio01_Biblioteca.dll");
var hoy = DateTime.Today;
var resultados = new List<(string Caso, string Esperado, string Real, bool Correcto)>();

Datos Base() => new()
{
    Usuarios = [new() { Id = 1, Nombre = "Ana" }],
    Materiales =
    [
        new() { Id = 1, Titulo = "Libro A", Tipo = "Libro", Total = 5 },
        new() { Id = 2, Titulo = "Revista B", Tipo = "Revista", Total = 1 }
    ]
};

Prestamo Prestamo(int id = 1, int material = 1, int vence = -2) => new()
{
    Id = id, UsuarioId = 1, MaterialId = material,
    Fecha = hoy.AddDays(-10), Vence = hoy.AddDays(vence)
};

(Datos Datos, string Salida) Ejecutar(Datos inicial, params string[] entradas)
{
    string carpeta = Path.Combine(AppContext.BaseDirectory, "casos-temporales", Guid.NewGuid().ToString());
    Directory.CreateDirectory(carpeta);
    string archivo = Path.Combine(carpeta, "datos.json");
    try
    {
        File.WriteAllText(archivo, JsonSerializer.Serialize(inicial));
        var inicio = new ProcessStartInfo("dotnet")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        inicio.ArgumentList.Add(programa);
        inicio.ArgumentList.Add(archivo);
        using var proceso = Process.Start(inicio) ?? throw new Exception("No se pudo iniciar la biblioteca.");
        foreach (string entrada in entradas) proceso.StandardInput.WriteLine(entrada);
        proceso.StandardInput.WriteLine("0");
        proceso.StandardInput.Close();
        string salida = proceso.StandardOutput.ReadToEnd();
        string errores = proceso.StandardError.ReadToEnd();
        if (!proceso.WaitForExit(15000))
        {
            proceso.Kill();
            throw new Exception("El programa tardó demasiado.");
        }
        if (proceso.ExitCode != 0) throw new Exception(errores);
        Datos final = JsonSerializer.Deserialize<Datos>(File.ReadAllText(archivo)) ?? throw new Exception("No se leyeron los datos.");
        return (final, salida);
    }
    finally { Directory.Delete(carpeta, true); }
}

void Comprobar(bool condicion, string mensaje = "El resultado no coincide.")
{
    if (!condicion) throw new Exception(mensaje);
}

bool Iguales(Datos a, Datos b) => JsonSerializer.Serialize(a) == JsonSerializer.Serialize(b);

void Caso(string nombre, string esperado, Action prueba)
{
    try
    {
        prueba();
        resultados.Add((nombre, esperado, "Cumple", true));
        Console.WriteLine("OK " + nombre);
    }
    catch (Exception e)
    {
        resultados.Add((nombre, esperado, e.Message.Replace('|', '/').Replace('\n', ' '), false));
        Console.WriteLine("ERROR " + nombre + ": " + e.Message);
    }
}

void Plazo(int material, int dias)
{
    var (datos, _) = Ejecutar(Base(), "3", "1", material.ToString());
    Comprobar(datos.Prestamos.Single().Vence.Date == hoy.AddDays(dias));
}

Caso("Plazo de libro", "Vence dentro de 7 días", () => Plazo(1, 7));
Caso("Plazo de revista", "Vence dentro de 3 días", () => Plazo(2, 3));

Caso("Cuarto préstamo", "Se rechaza sin crear otro préstamo", () =>
{
    Datos datos = Base();
    datos.Prestamos = [Prestamo(1), Prestamo(2), Prestamo(3)];
    var (final, salida) = Ejecutar(datos, "3", "1", "1");
    Comprobar(final.Prestamos.Count == 3 && salida.Contains("Ya tiene 3"));
});

Caso("Sin ejemplares", "Se rechaza y no modifica datos", () =>
{
    Datos datos = Base();
    datos.Prestamos = [Prestamo(material: 2)];
    var (final, salida) = Ejecutar(datos, "3", "1", "2");
    Comprobar(Iguales(final, datos) && salida.Contains("No hay ejemplares"));
});

Caso("Usuario con deuda", "No obtiene nuevos préstamos", () =>
{
    Datos datos = Base();
    datos.Usuarios[0].MultaPendiente = 1;
    var (final, salida) = Ejecutar(datos, "3", "1", "1");
    Comprobar(Iguales(final, datos) && salida.Contains("multas pendientes"));
});

void Devolver(int material, int retraso, decimal multa)
{
    Datos datos = Base();
    datos.Prestamos = [Prestamo(material: material, vence: -retraso)];
    var (final, salida) = Ejecutar(datos, "4", "1", "4", "1", "11");
    Comprobar(final.Prestamos[0].Multa == multa &&
              final.Usuarios[0].MultaPendiente == multa &&
              final.Prestamos[0].Devuelto?.Date == hoy &&
              salida.Contains("Ya fue devuelto"));
}

Caso("Libro con 2 días de retraso y devolución repetida", "$0,50 una sola vez", () => Devolver(1, 2, .5m));
Caso("Revista con 2 días de retraso y devolución repetida", "$1,00 una sola vez", () => Devolver(2, 2, 1m));
Caso("Devolución en fecha límite", "Multa cero", () => Devolver(1, 0, 0m));

Caso("Pagos y desbloqueo", "Rechaza montos inválidos y permite prestar al saldar", () =>
{
    Datos datos = Base();
    datos.Usuarios[0].MultaPendiente = 2;
    var (final, salida) = Ejecutar(datos, "5", "1", "-1", "5", "1", "3", "5", "1", "1", "5", "1", "1", "3", "1", "1");
    Comprobar(final.Usuarios[0].MultaPendiente == 0 && final.Prestamos.Count == 1 &&
              salida.Split("Pago inv").Length - 1 == 2);
});

Caso("Vencidos, historial, más prestados y auditoría", "Listados y cantidades consistentes", () =>
{
    Datos datos = Base();
    datos.Prestamos = [Prestamo(), Prestamo(2, 1, 3)];
    var (_, salida) = Ejecutar(datos, "7", "8", "1", "9", "11");
    Comprobar(salida.Contains("#1 Ana - Libro A") && salida.Contains("#2 Ana - Libro A") &&
              salida.Contains("Libro A: 2 pr") && salida.Contains("total 5, prestados 2, disponibles 3"));
    var (_, vencidos) = Ejecutar(datos, "7");
    Comprobar(vencidos.Contains("#1 Ana") && !vencidos.Contains("#2 Ana"));
});

Caso("Multas por período", "Incluye ambos límites y rechaza período invertido", () =>
{
    Datos datos = Base();
    Prestamo p = Prestamo();
    p.Devuelto = hoy;
    p.Multa = .5m;
    datos.Prestamos = [p];
    datos.Usuarios[0].MultaPendiente = .5m;
    var (_, salida) = Ejecutar(datos, "10", hoy.ToString("yyyy-MM-dd"), hoy.ToString("yyyy-MM-dd"));
    Comprobar(salida.Contains("0,50") || salida.Contains("0.50"));
    var (_, invertido) = Ejecutar(datos, "10", hoy.AddDays(1).ToString("yyyy-MM-dd"), hoy.ToString("yyyy-MM-dd"));
    Comprobar(invertido.Contains("Fechas inv"));
});

Caso("IDs y cantidades inválidas", "No modifica datos", () =>
{
    Datos datos = Base();
    var (final, _) = Ejecutar(datos, "3", "999", "1", "4", "999", "2", "Inválido", "1", "0");
    Comprobar(Iguales(final, datos));
});

Caso("Persistencia", "Conserva usuarios y préstamos al volver a iniciar", () =>
{
    var (datos, _) = Ejecutar(Base(), "1", "Luis", "3", "2", "1");
    var (final, salida) = Ejecutar(datos, "6", "8", "2");
    Comprobar(salida.Contains("Luis") && salida.Contains("#1 Luis") && Iguales(final, datos));
});

string informe = "# Resultados de pruebas: ejercicio 1\n\n" +
                 $"Ejecución real: {hoy:yyyy-MM-dd}. Pruebas aisladas mediante el menú y archivos JSON temporales.\n\n" +
                 "| Caso | Resultado esperado | Resultado real | Estado |\n|---|---|---|---|\n";
foreach (var fila in resultados)
    informe += $"| {fila.Caso} | {fila.Esperado} | {fila.Real} | {(fila.Correcto ? "Correcto" : "Falló")} |\n";
File.WriteAllText(Path.Combine(raiz, "documentacion", "informe", "Pruebas-Ejercicio01.md"), informe);
Console.WriteLine($"{resultados.Count(x => x.Correcto)}/{resultados.Count} pruebas correctas.");
if (resultados.Any(x => !x.Correcto)) Environment.ExitCode = 1;
