namespace Ejercicio05_Artesanias.Modelos;

public class DetalleDevolucion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DetalleRecepcionId { get; set; } = string.Empty;
    public int CantidadDevuelta { get; set; }
}
