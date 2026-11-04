using System.Globalization;
using System.Text.Json;
using Ejercicio02_CooperativaAhorro.Modelos;
using Ejercicio02_CooperativaAhorro.Servicios;
using Ejercicio02_CooperativaAhorro.Persistencia;
using Ejercicio02_CooperativaAhorro.Reportes;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
var dia = new DateOnly(2026, 10, 1);
string temporal = Path.Combine(Path.GetTempPath(), "pruebas-cooperativa-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(temporal);
var resultados = new List<(string Caso, string Esperado, string Real, bool Ok)>();
int contador = 0;
Cooperativa Nueva()
{
    var c = new Cooperativa(Path.Combine(temporal, (++contador) + ".json"));
    c.AgregarSocio(1, "Ana");
    c.AgregarSocio(2, "Luis");
    c.AbrirCuenta(101, 1, true, dia);
    c.AbrirCuenta(102, 1, false, dia);
    c.AbrirCuenta(201, 2, false, dia);
    c.Depositar(101, 100m, dia);
    return c;
}

void Igual<T>(T esperado, T real)
{
    if (!EqualityComparer<T>.Default.Equals(esperado, real))
    {
        throw new Exception($"Esperado {esperado}; obtenido {real}");
    }
}

void Rechaza(Action accion)
{
    try
    {
        accion();
    }
    catch (InvalidOperationException)
    {
        return;
    }

    throw new Exception("Se acepto una operacion que debia rechazarse.");
}

decimal Saldo(Cooperativa c, int n)
{
    return c.Consultar().Cuentas.Single(x => x.Numero == n).Saldo;
}

string Foto(Cooperativa c)
{
    return JsonSerializer.Serialize(c.Consultar());
}

void Caso(string nombre, string esperado, Func<string> prueba)
{
    try
    {
        var real = prueba();
        resultados.Add((nombre, esperado, real, true));
        Console.WriteLine("OK " + nombre);
    }
    catch (Exception e)
    {
        resultados.Add((nombre, esperado, e.Message, false));
        Console.WriteLine("ERROR " + nombre + ": " + e.Message);
    }
}

try
{
    Caso("Socio con varias cuentas", "2 cuentas para Ana", () =>
    {
        var c = Nueva();
        Igual(2, c.Consultar().Cuentas.Count(x => x.SocioId == 1));
        return "2 cuentas";
    });
    Caso("Socio repetido", "Rechazo", () =>
    {
        var c = Nueva();
        Rechaza(() => c.AgregarSocio(1, "Otro"));
        return "Rechazado";
    });
    Caso("Datos de socio incompletos", "Rechazo", () =>
    {
        var c = Nueva();
        Rechaza(() => c.AgregarSocio(0, ""));
        return "Rechazado";
    });
    Caso("Cuenta sin socio", "Rechazo", () =>
    {
        var c = Nueva();
        Rechaza(() => c.AbrirCuenta(999, 9, true, dia));
        return "Rechazado";
    });
    Caso("Numero de cuenta repetido", "Rechazo", () =>
    {
        var c = Nueva();
        Rechaza(() => c.AbrirCuenta(101, 1, false, dia));
        return "Rechazado";
    });
    Caso("Deposito valido", "Saldo 125.50", () =>
    {
        var c = Nueva();
        c.Depositar(101, 25.5m, dia);
        Igual(125.5m, Saldo(c, 101));
        return "125.50";
    });
    foreach (decimal monto in new[]
    {
        0m,
        -1m,
        1.001m
    }

    )
    {
        decimal v = monto;
        Caso("Deposito invalido " + v, "Rechazo sin cambios", () =>
        {
            var c = Nueva();
            string antes = Foto(c);
            Rechaza(() => c.Depositar(101, v, dia));
            Igual(antes, Foto(c));
            return "Rechazado; estado intacto";
        });
        Caso("Retiro invalido " + v, "Rechazo sin cambios", () =>
        {
            var c = Nueva();
            string antes = Foto(c);
            Rechaza(() => c.Retirar(101, v, dia));
            Igual(antes, Foto(c));
            return "Rechazado; estado intacto";
        });
    }

    Caso("Primer deposito de ahorro menor a 5", "Rechazo", () =>
    {
        var c = Nueva();
        c.AbrirCuenta(103, 1, true, dia);
        Rechaza(() => c.Depositar(103, 4.99m, dia));
        return "Rechazado";
    });
    Caso("Primer deposito de ahorro igual a 5", "Saldo 5", () =>
    {
        var c = Nueva();
        c.AbrirCuenta(103, 1, true, dia);
        c.Depositar(103, 5, dia);
        Igual(5m, Saldo(c, 103));
        return "5.00";
    });
    Caso("Retiro deja minimo de ahorro", "Saldo 5", () =>
    {
        var c = Nueva();
        c.Retirar(101, 95, dia);
        Igual(5m, Saldo(c, 101));
        return "5.00";
    });
    Caso("Retiro rompe minimo de ahorro", "Rechazo; saldo 100", () =>
    {
        var c = Nueva();
        Rechaza(() => c.Retirar(101, 95.01m, dia));
        Igual(100m, Saldo(c, 101));
        return "Rechazado; 100.00";
    });
    Caso("Sobregiro exacto", "Saldo -200", () =>
    {
        var c = Nueva();
        c.Retirar(102, 200, dia);
        Igual(-200m, Saldo(c, 102));
        return "-200.00";
    });
    Caso("Sobregiro excedido", "Rechazo", () =>
    {
        var c = Nueva();
        Rechaza(() => c.Retirar(102, 200.01m, dia));
        Igual(0m, Saldo(c, 102));
        return "Rechazado; 0.00";
    });
    Caso("Transferencia valida y vinculada", "70 y 30; retiro/deposito con mismo id", () =>
    {
        var c = Nueva();
        c.Transferir(1, 101, 102, 30, dia);
        Igual(70m, Saldo(c, 101));
        Igual(30m, Saldo(c, 102));
        var m = c.Consultar().Movimientos.TakeLast(2).ToArray();
        Igual("Retiro", m[0].Tipo);
        Igual("Deposito", m[1].Tipo);
        Igual(m[0].TransferenciaId, m[1].TransferenciaId);
        if (m[0].TransferenciaId is null)
        {
            throw new Exception("Falta vinculo");
        }

        return "70.00 / 30.00; vinculada";
    });
    Caso("Transferencia misma cuenta", "Rechazo sin cambios", () =>
    {
        var c = Nueva();
        var f = Foto(c);
        Rechaza(() => c.Transferir(1, 101, 101, 10, dia));
        Igual(f, Foto(c));
        return "Rechazado; intacto";
    });
    Caso("Transferencia entre socios distintos", "Rechazo", () =>
    {
        var c = Nueva();
        Rechaza(() => c.Transferir(1, 101, 201, 10, dia));
        return "Rechazado";
    });
    Caso("Transferencia sin fondos", "Ninguna cuenta cambia", () =>
    {
        var c = Nueva();
        var f = Foto(c);
        Rechaza(() => c.Transferir(1, 101, 102, 96, dia));
        Igual(f, Foto(c));
        return "Estado y movimientos intactos";
    });
    Caso("Transferencia a cuenta inexistente", "Ninguna cuenta cambia", () =>
    {
        var c = Nueva();
        var f = Foto(c);
        Rechaza(() => c.Transferir(1, 101, 999, 10, dia));
        Igual(f, Foto(c));
        return "Estado intacto";
    });
    Caso("Transferencia con destino bajo minimo", "Ninguna cuenta cambia", () =>
    {
        var c = Nueva();
        c.AbrirCuenta(103, 1, true, dia);
        var f = Foto(c);
        Rechaza(() => c.Transferir(1, 102, 103, 4, dia));
        Igual(f, Foto(c));
        return "Estado intacto";
    });
    Caso("Fallo de segunda operacion por fecha", "Retiro de origen tampoco se conserva", () =>
    {
        var c = Nueva();
        c.Depositar(102, 10, dia.AddDays(2));
        var f = Foto(c);
        Rechaza(() => c.Transferir(1, 101, 102, 10, dia));
        Igual(f, Foto(c));
        return "Estado intacto";
    });
    Caso("Interes anual 4 por ciento mensual", "0.33 en ahorro; 0 en corriente", () =>
    {
        var c = Nueva();
        c.AplicarIntereses(2026, 10);
        Igual(100.33m, Saldo(c, 101));
        Igual(0m, Saldo(c, 102));
        return "Interes 0.33; corriente 0.00";
    });
    Caso("Interes repetido", "No se duplica", () =>
    {
        var c = Nueva();
        c.AplicarIntereses(2026, 10);
        string f = Foto(c);
        c.AplicarIntereses(2026, 10);
        Igual(f, Foto(c));
        return "Sin cambios al repetir";
    });
    Caso("Interes compuesto mes siguiente", "Saldo 100.66", () =>
    {
        var c = Nueva();
        c.AplicarIntereses(2026, 10);
        c.AplicarIntereses(2026, 11);
        Igual(100.66m, Saldo(c, 101));
        return "100.66";
    });
    Caso("Cuenta futura sin interes", "Sin abono anterior a apertura", () =>
    {
        var c = Nueva();
        c.AbrirCuenta(103, 1, true, new(2026, 12, 1));
        c.AplicarIntereses(2026, 10);
        Igual(0, c.Consultar().Movimientos.Count(m => m.CuentaNumero == 103));
        return "0 movimientos";
    });
    Caso("Cerrar saldo distinto de cero", "Rechazo", () =>
    {
        var c = Nueva();
        Rechaza(() => c.CerrarCuenta(101));
        return "Rechazado";
    });
    Caso("Cerrar sobregiro", "Rechazo", () =>
    {
        var c = Nueva();
        c.Retirar(102, 10, dia);
        Rechaza(() => c.CerrarCuenta(102));
        return "Rechazado";
    });
    Caso("Cerrar sin movimientos", "Eliminada", () =>
    {
        var c = Nueva();
        c.CerrarCuenta(102);
        Igual(false, c.Consultar().Cuentas.Any(x => x.Numero == 102));
        return "Eliminada";
    });
    Caso("Cerrar con movimientos y saldo cero", "Conservada e inactiva", () =>
    {
        var c = Nueva();
        c.Depositar(102, 10, dia);
        c.Retirar(102, 10, dia);
        c.CerrarCuenta(102);
        Igual(false, c.Consultar().Cuentas.Single(x => x.Numero == 102).Activa);
        Igual(2, c.Consultar().Movimientos.Count(x => x.CuentaNumero == 102));
        return "Inactiva; 2 movimientos conservados";
    });
    Caso("Operar cuenta inactiva", "Deposito, retiro y transferencia rechazados", () =>
    {
        var c = Nueva();
        c.Depositar(102, 10, dia);
        c.Retirar(102, 10, dia);
        c.CerrarCuenta(102);
        var f = Foto(c);
        Rechaza(() => c.Depositar(102, 10, dia));
        Rechaza(() => c.Retirar(102, 10, dia));
        Rechaza(() => c.Transferir(1, 101, 102, 10, dia));
        Igual(f, Foto(c));
        return "3 rechazos; estado intacto";
    });
    Caso("Fecha anterior a apertura", "Rechazo", () =>
    {
        var c = Nueva();
        Rechaza(() => c.Depositar(101, 10, dia.AddDays(-1)));
        return "Rechazado";
    });
    Caso("Movimiento despues de cierre mensual", "Rechaza fecha del mes cerrado", () =>
    {
        var c = Nueva();
        c.AplicarIntereses(2026, 10);
        Rechaza(() => c.Depositar(101, 10, new(2026, 10, 31)));
        return "Rechazado";
    });
    Caso("Estado de cuenta", "Saldos 100, 110, 105", () =>
    {
        var c = Nueva();
        c.Depositar(101, 10, dia);
        c.Retirar(101, 5, dia);
        var m = ReportesCooperativa.EstadoCuenta(c.Consultar(), 101);
        Igual("100,110,105", string.Join(',', m.Select(x => x.SaldoPosterior)));
        return "100,110,105";
    });
    Caso("Reporte intereses historico", "Calculado y acreditado 0.33", () =>
    {
        var c = Nueva();
        c.AplicarIntereses(2026, 10);
        c.Depositar(101, 100, new(2026, 11, 1));
        var r = ReportesCooperativa.InteresesMes(c.Consultar(), 2026, 10).Single();
        Igual(0.33m, r.Calculado);
        Igual(0.33m, r.Acreditado);
        return "0.33 / 0.33";
    });
    Caso("Reporte sobregiros", "Solo cuenta 102", () =>
    {
        var c = Nueva();
        c.Retirar(102, 25, dia);
        Igual(102, ReportesCooperativa.Sobregiros(c.Consultar()).Single().Numero);
        return "102: -25.00";
    });
    Caso("Totales e inclusion de transferencias", "Depositos 120; retiros 20; interes excluido", () =>
    {
        var c = Nueva();
        c.Transferir(1, 101, 102, 20, dia);
        c.AplicarIntereses(2026, 10);
        var r = ReportesCooperativa.Totales(c.Consultar(), dia, new(2026, 10, 31));
        Igual(120m, r.Depositado);
        Igual(20m, r.Retirado);
        return "120.00 / 20.00";
    });
    Caso("Limites de periodo inclusivos", "Solo 10 del dia consultado", () =>
    {
        var c = Nueva();
        c.Depositar(101, 10, dia.AddDays(1));
        var r = ReportesCooperativa.Totales(c.Consultar(), dia.AddDays(1), dia.AddDays(1));
        Igual(10m, r.Depositado);
        return "10.00";
    });
    Caso("Periodo invertido", "Rechazo", () =>
    {
        var c = Nueva();
        Rechaza(() => ReportesCooperativa.Totales(c.Consultar(), dia.AddDays(1), dia));
        return "Rechazado";
    });
    Caso("Auditoria correcta", "Todas las cuentas correctas", () =>
    {
        var c = Nueva();
        c.Transferir(1, 101, 102, 10, dia);
        c.AplicarIntereses(2026, 10);
        Igual(true, ReportesCooperativa.Auditar(c.Consultar()).All(a => a.Correcta));
        return "3 cuentas correctas";
    });
    Caso("Auditoria detecta saldo alterado", "Cuenta 101 incorrecta", () =>
    {
        var d = Nueva().Consultar();
        d.Cuentas[0].Saldo = 999;
        Igual(false, ReportesCooperativa.Auditar(d)[0].Correcta);
        return "Diferencia detectada";
    });
    Caso("Auditoria detecta saldo posterior alterado", "Historial incorrecto", () =>
    {
        var d = Nueva().Consultar();
        d.Movimientos[0].SaldoPosterior = 999;
        Igual(false, ReportesCooperativa.Auditar(d)[0].HistorialCorrecto);
        return "Historial alterado detectado";
    });
    Caso("Persistencia y tipos de cuenta", "Mismo estado al reabrir JSON", () =>
    {
        string ruta = Path.Combine(temporal, "reabrir.json");
        var c = new Cooperativa(ruta);
        c.AgregarSocio(1, "Ana");
        c.AbrirCuenta(1, 1, true, dia);
        c.Depositar(1, 5, dia);
        var otro = new Cooperativa(ruta);
        Igual(Foto(c), Foto(otro));
        Igual(true, otro.Consultar().Cuentas[0] is CuentaAhorro);
        return "JSON y tipo conservados";
    });
    Caso("Archivo JSON danado", "Error; contenido conservado", () =>
    {
        string r = Path.Combine(temporal, "roto.json");
        File.WriteAllText(r, "roto");
        bool error = false;
        try
        {
            new Cooperativa(r);
        }
        catch (JsonException)
        {
            error = true;
        }

        Igual(true, error);
        Igual("roto", File.ReadAllText(r));
        return "Error de lectura; sin sobrescritura";
    });
    Caso("Error al guardar transferencia", "Archivo previo y memoria intactos", () =>
    {
        string r = Path.Combine(temporal, "bloqueo.json");
        var c = new Cooperativa(r);
        c.AgregarSocio(1, "Ana");
        c.AbrirCuenta(1, 1, false, dia);
        c.AbrirCuenta(2, 1, false, dia);
        c.Depositar(1, 20, dia);
        string antes = Foto(c), contenido = File.ReadAllText(r);
        File.Move(r, r + ".bak");
        Directory.CreateDirectory(r);
        bool fallo = false;
        try
        {
            c.Transferir(1, 1, 2, 10, dia);
        }
        catch (Exception e)when (e is IOException or UnauthorizedAccessException)
        {
            fallo = true;
        }

        Igual(true, fallo);
        Igual(antes, Foto(c));
        Igual(contenido, File.ReadAllText(r + ".bak"));
        return "Fallo controlado; memoria y respaldo intactos";
    });
    if (args.Length >= 1)
    {
        string salida = Path.GetFullPath(args[0]);
        Directory.CreateDirectory(Path.GetDirectoryName(salida)!);
        var lineas = new List<string>
        {
            "# Resultados reales - Ejercicio 2",
            "",
            "Ejecucion: " + DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm zzz"),
            "",
            "Comando: `dotnet run --project pruebas/Ejercicio02_CooperativaAhorro -- pruebas/Ejercicio02_CooperativaAhorro/RESULTADOS.md`",
            "",
            "| Caso | Esperado | Resultado real | Estado |",
            "| --- | --- | --- | --- |"
        };
        lineas.AddRange(resultados.Select(r => $"| {r.Caso} | {r.Esperado} | {r.Real.Replace('|', '/')} | {(r.Ok ? "OK" : "FALLO")} |"));
        lineas.Add($"\nTotal: {resultados.Count}; correctos: {resultados.Count(r => r.Ok)}; fallidos: {resultados.Count(r => !r.Ok)}.");
        File.WriteAllLines(salida, lineas);
    }

    if (args.Length >= 2)
    {
        string ejemplo = Path.GetFullPath(args[1]);
        if (File.Exists(ejemplo))
        {
            throw new Exception("No se sobrescribe un archivo de ejemplo existente.");
        }

        var c = new Cooperativa(ejemplo);
        c.AgregarSocio(1, "Ana Torres");
        c.AgregarSocio(2, "Luis Mina");
        c.AbrirCuenta(101, 1, true, dia);
        c.AbrirCuenta(102, 1, false, dia);
        c.AbrirCuenta(201, 2, true, dia);
        c.Depositar(101, 100, dia);
        c.Depositar(201, 300, dia);
        c.Transferir(1, 101, 102, 20, dia.AddDays(1));
        c.Retirar(102, 50, dia.AddDays(2));
        c.AplicarIntereses(2026, 10);
        Igual(true, ReportesCooperativa.Auditar(c.Consultar()).All(x => x.Correcta));
        Console.WriteLine("Datos de ejemplo generados y auditados: " + ejemplo);
    }
}
finally
{
    Directory.Delete(temporal, true);
}

Console.WriteLine($"{resultados.Count(r => r.Ok)}/{resultados.Count} pruebas correctas.");
return resultados.All(r => r.Ok) ? 0 : 1;
