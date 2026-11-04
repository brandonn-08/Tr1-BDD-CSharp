namespace Ejercicio05_Artesanias.Modelos;

public class Emprendedor
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ComunidadId { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
