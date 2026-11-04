using System.Text.Json.Serialization;

namespace Ejercicio02_CooperativaAhorro.Modelos
{
    public class DatosCooperativa
    {
        public List<Socio> Socios { get; set; } = [];
        public List<Cuenta> Cuentas { get; set; } = [];
        public List<Movimiento> Movimientos { get; set; } = [];
    }
}
