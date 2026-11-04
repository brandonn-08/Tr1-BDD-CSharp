using System.Text.Json.Serialization;

namespace Ejercicio02_CooperativaAhorro.Modelos
{
    public class CuentaAhorro : Cuenta
    {
        public override bool PermiteSaldo(decimal nuevoSaldo)
        {
            return nuevoSaldo >= 5m;
        }

        public override decimal InteresMensual()
        {
            return decimal.Round(Saldo * 0.04m / 12m, 2, MidpointRounding.AwayFromZero);
        }
    }
}
