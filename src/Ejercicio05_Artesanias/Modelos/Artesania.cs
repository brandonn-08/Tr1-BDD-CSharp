namespace Ejercicio05_Artesanias.Modelos;

public class Artesania
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmprendedorId { get; set; } = string.Empty;
    public string TipoArtesaniaId { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal PrecioVenta { get; set; }
    public bool Activa { get; set; } = true;
}
