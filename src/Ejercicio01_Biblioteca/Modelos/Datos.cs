namespace Biblioteca.Modelos;

public class Datos
{
    public List<Usuario> Usuarios { get; set; } = new();
    public List<Material> Materiales { get; set; } = new();
    public List<Prestamo> Prestamos { get; set; } = new();
}
