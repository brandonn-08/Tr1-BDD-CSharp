using Ejercicio02_CooperativaAhorro.Modelos;

namespace Ejercicio02_CooperativaAhorro.Reportes
{
    public record InteresCuenta(int Cuenta, decimal Saldo, decimal Calculado, decimal Acreditado, bool Aplicado);
    public record TotalesPeriodo(decimal Depositado, decimal Retirado);
    public record AuditoriaCuenta(int Cuenta, decimal Guardado, decimal Calculado, bool HistorialCorrecto)
    {
        public bool Correcta => Guardado == Calculado && HistorialCorrecto;
    }

    public static class ReportesCooperativa
    {
        public static List<Movimiento> EstadoCuenta(DatosCooperativa d, int numero)
        {
            if (!d.Cuentas.Any(c => c.Numero == numero))
            {
                throw new InvalidOperationException("La cuenta no existe.");
            }

            return d.Movimientos.Where(m => m.CuentaNumero == numero).OrderBy(m => m.Id).ToList();
        }

        public static List<InteresCuenta> InteresesMes(DatosCooperativa d, int anio, int mes)
        {
            var fin = new DateOnly(anio, mes, DateTime.DaysInMonth(anio, mes));
            return d.Cuentas.OfType<CuentaAhorro>().Where(c => c.Apertura <= fin).Select(c =>
            {
                var movimientos = d.Movimientos.Where(m => m.CuentaNumero == c.Numero && m.Fecha <= fin).ToList();
                decimal saldo = movimientos.Sum(m => m.Tipo == "Retiro" ? -m.Monto : m.Monto);
                var intereses = movimientos.Where(m => m.Tipo == "Interes" && m.Fecha.Year == anio && m.Fecha.Month == mes).ToList();
                decimal baseCalculo = saldo - intereses.Sum(m => m.Monto);
                decimal calculado = decimal.Round(baseCalculo * 0.04m / 12m, 2, MidpointRounding.AwayFromZero);
                return new InteresCuenta(c.Numero, baseCalculo, calculado, intereses.Sum(m => m.Monto), intereses.Count > 0);
            }).ToList();
        }

        public static List<Cuenta> Sobregiros(DatosCooperativa d)
        {
            return d.Cuentas.Where(c => c.Saldo < 0).ToList();
        }

        public static TotalesPeriodo Totales(DatosCooperativa d, DateOnly desde, DateOnly hasta)
        {
            if (desde > hasta)
            {
                throw new InvalidOperationException("El inicio no puede superar al fin.");
            }

            var m = d.Movimientos.Where(m => m.Fecha >= desde && m.Fecha <= hasta).ToList();
            return new(m.Where(m => m.Tipo == "Deposito").Sum(m => m.Monto), m.Where(m => m.Tipo == "Retiro").Sum(m => m.Monto));
        }

        public static List<AuditoriaCuenta> Auditar(DatosCooperativa d)
        {
            return d.Cuentas.Select(c =>
            {
                decimal saldo = 0;
                bool historial = true;
                foreach (var m in d.Movimientos.Where(m => m.CuentaNumero == c.Numero).OrderBy(m => m.Id))
                {
                    saldo += m.Tipo == "Retiro" ? -m.Monto : m.Monto;
                    historial &= saldo == m.SaldoPosterior;
                }

                return new AuditoriaCuenta(c.Numero, c.Saldo, saldo, historial);
            }).ToList();
        }
    }
}
