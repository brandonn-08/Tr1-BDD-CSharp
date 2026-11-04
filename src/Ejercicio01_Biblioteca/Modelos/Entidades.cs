namespace Biblioteca.Modelos;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public decimal MultaPendiente { get; set; }
}

public class Material
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string Tipo { get; set; } = "";
    public int Total { get; set; }
}

public class Prestamo
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int MaterialId { get; set; }
    public DateTime Fecha { get; set; }
    public DateTime Vence { get; set; }
    public DateTime? Devuelto { get; set; }
    public decimal Multa { get; set; }
}

public class Datos
{
    public List<Usuario> Usuarios { get; set; } = new();
    public List<Material> Materiales { get; set; } = new();
    public List<Prestamo> Prestamos { get; set; } = new();
}
