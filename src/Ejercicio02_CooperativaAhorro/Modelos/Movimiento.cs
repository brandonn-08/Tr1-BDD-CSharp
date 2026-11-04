using System.Text.Json.Serialization;

namespace Ejercicio02_CooperativaAhorro.Modelos
{
    public class Movimiento
    {
        public int Id { get; set; }
        public int CuentaNumero { get; set; }
        public DateOnly Fecha { get; set; }
        public string Tipo { get; set; } = ""; // Deposito, Retiro o Interes
        public decimal Monto { get; set; }
        public decimal SaldoPosterior { get; set; }
        public string Detalle { get; set; } = "";
        public string? TransferenciaId { get; set; }
    }
}
