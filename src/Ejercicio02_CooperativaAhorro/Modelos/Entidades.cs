using System.Text.Json.Serialization;

namespace Ejercicio02_CooperativaAhorro.Modelos;

public class Socio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
}

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

public class CuentaAhorro : Cuenta
{
    public override bool PermiteSaldo(decimal nuevoSaldo) => nuevoSaldo >= 5m;
    public override decimal InteresMensual() => decimal.Round(Saldo * 0.04m / 12m, 2, MidpointRounding.AwayFromZero);
}

public class CuentaCorriente : Cuenta
{
    public override bool PermiteSaldo(decimal nuevoSaldo) => nuevoSaldo >= -200m;
    public override decimal InteresMensual() => 0m;
}

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

public class DatosCooperativa
{
    public List<Socio> Socios { get; set; } = [];
    public List<Cuenta> Cuentas { get; set; } = [];
    public List<Movimiento> Movimientos { get; set; } = [];
}
