namespace Ejercicio05_Artesanias.Modelos;

public class DetalleRecepcion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ArtesaniaId { get; set; } = string.Empty;
    public int CantidadRecibida { get; set; }
    public decimal PrecioUnitario { get; set; }
}
