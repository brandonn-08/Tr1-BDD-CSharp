namespace Ejercicio05_Artesanias.Modelos;

public class Devolucion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmprendedorId { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string Observacion { get; set; } = string.Empty;

    public List<DetalleDevolucion> Detalles { get; set; } = new();
}
