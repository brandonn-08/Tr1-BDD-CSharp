using System.Text.Json.Serialization;

namespace Ejercicio02_CooperativaAhorro.Modelos
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "tipo")]
    [JsonDerivedType(typeof(CuentaAhorro), "ahorro")]
    [JsonDerivedType(typeof(CuentaCorriente), "corriente")]
    public abstract class Cuenta
    {
        public int Numero { get; set; }
        public int SocioId { get; set; }
        public decimal Saldo { get; set; }
        public bool Activa { get; set; } = true;
        public DateOnly Apertura { get; set; }

        public abstract bool PermiteSaldo(decimal nuevoSaldo);
        public abstract decimal InteresMensual();
    }
}
