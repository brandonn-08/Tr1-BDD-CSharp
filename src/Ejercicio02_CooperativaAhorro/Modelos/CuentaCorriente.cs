using System.Text.Json.Serialization;

namespace Ejercicio02_CooperativaAhorro.Modelos
{
    public class CuentaCorriente : Cuenta
    {
        public override bool PermiteSaldo(decimal nuevoSaldo)
        {
            return nuevoSaldo >= -200m;
        }

        public override decimal InteresMensual()
        {
            return 0m;
        }
    }
}
