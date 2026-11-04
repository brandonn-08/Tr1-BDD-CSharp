namespace Ejercicio05_Artesanias.Modelos;

public class Venta
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string Observacion { get; set; } = string.Empty;

    public List<DetalleVenta> Detalles { get; set; } = new();
}
