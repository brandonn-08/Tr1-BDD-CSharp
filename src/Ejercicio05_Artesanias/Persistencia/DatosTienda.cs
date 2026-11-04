using Ejercicio05_Artesanias.Modelos;

namespace Ejercicio05_Artesanias.Persistencia;

public class DatosTienda
{
    public List<Comunidad> Comunidades { get; set; } = new();

    public List<Emprendedor> Emprendedores { get; set; } = new();

    public List<TipoArtesania> TiposArtesanias { get; set; } = new();

    public List<Artesania> Artesanias { get; set; } = new();

    public List<Recepcion> Recepciones { get; set; } = new();

    public List<Venta> Ventas { get; set; } = new();

    public List<Devolucion> Devoluciones { get; set; } = new();
}
