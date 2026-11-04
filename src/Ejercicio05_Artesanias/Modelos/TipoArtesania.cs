namespace Ejercicio05_Artesanias.Modelos;

public class TipoArtesania
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Nombre { get; set; } = string.Empty;
    public decimal PorcentajeComision { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
