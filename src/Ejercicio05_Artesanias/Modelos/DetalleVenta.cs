namespace Ejercicio05_Artesanias.Modelos;

public class DetalleVenta
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DetalleRecepcionId { get; set; } = string.Empty;
    public int CantidadVendida { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal PorcentajeComision { get; set; }

    public decimal TotalVendido =>
        CantidadVendida * PrecioUnitario;

    public decimal ComisionTienda =>
        decimal.Round(TotalVendido * PorcentajeComision / 100m, 2, MidpointRounding.AwayFromZero);

    public decimal MontoEmprendedor =>
        TotalVendido - ComisionTienda;
}
