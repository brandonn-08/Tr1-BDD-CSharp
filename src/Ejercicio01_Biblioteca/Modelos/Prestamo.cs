namespace Biblioteca.Modelos;

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
