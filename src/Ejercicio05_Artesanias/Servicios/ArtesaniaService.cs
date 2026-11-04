using Ejercicio05_Artesanias.Modelos;
using Ejercicio05_Artesanias.Persistencia;

namespace Ejercicio05_Artesanias.Servicios;

public class ArtesaniaService
{
    private readonly DatosTienda _datos;
    private readonly JsonRepository _repositorio;

    public ArtesaniaService(DatosTienda datos, JsonRepository repositorio)
    {
        _datos = datos;
        _repositorio = repositorio;
    }

    public void Registrar(
        string emprendedorId,
        string tipoArtesaniaId,
        string nombre,
        string descripcion,
        decimal precioVenta)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException(
                "El nombre de la artesanía es obligatorio."
            );
        }

        if (precioVenta <= 0)
        {
            throw new ArgumentException(
                "El precio de venta debe ser mayor que cero."
            );
        }

        bool emprendedorValido = _datos.Emprendedores.Any(e =>
            e.Id == emprendedorId && e.Activo
        );

        if (!emprendedorValido)
        {
            throw new InvalidOperationException(
                "El emprendedor no existe o está inactivo."
            );
        }

        bool tipoValido = _datos.TiposArtesanias.Any(t =>
            t.Id == tipoArtesaniaId && t.Activo
        );

        if (!tipoValido)
        {
            throw new InvalidOperationException(
                "El tipo de artesanía no existe o está inactivo."
            );
        }

        Artesania artesania = new()
        {
            EmprendedorId = emprendedorId,
            TipoArtesaniaId = tipoArtesaniaId,
            Nombre = nombre.Trim(),
            Descripcion = descripcion.Trim(),
            PrecioVenta = precioVenta
        };

        _datos.Artesanias.Add(artesania);

        _repositorio.Guardar(_datos);
    }

    public List<Artesania> Listar()
    {
        return _datos.Artesanias
            .OrderBy(a => a.Nombre)
            .ToList();
    }

    public Artesania BuscarPorId(string id)
    {
        return _datos.Artesanias.FirstOrDefault(a => a.Id == id)
            ?? throw new InvalidOperationException(
                "No se encontró la artesanía."
            );
    }

    public void Actualizar(
        string id,
        string emprendedorId,
        string tipoArtesaniaId,
        string nombre,
        string descripcion,
        decimal precioVenta)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException(
                "El nombre de la artesanía es obligatorio."
            );
        }

        if (precioVenta <= 0)
        {
            throw new ArgumentException(
                "El precio de venta debe ser mayor que cero."
            );
        }

        Artesania artesania = BuscarPorId(id);

        bool emprendedorValido = _datos.Emprendedores.Any(e =>
            e.Id == emprendedorId && e.Activo
        );

        if (!emprendedorValido)
        {
            throw new InvalidOperationException(
                "El emprendedor no existe o está inactivo."
            );
        }

        bool tipoValido = _datos.TiposArtesanias.Any(t =>
            t.Id == tipoArtesaniaId && t.Activo
        );

        if (!tipoValido)
        {
            throw new InvalidOperationException(
                "El tipo de artesanía no existe o está inactivo."
            );
        }

        artesania.EmprendedorId = emprendedorId;
        artesania.TipoArtesaniaId = tipoArtesaniaId;
        artesania.Nombre = nombre.Trim();
        artesania.Descripcion = descripcion.Trim();
        artesania.PrecioVenta = precioVenta;

        _repositorio.Guardar(_datos);
    }

    public void Desactivar(string id)
    {
        Artesania artesania = BuscarPorId(id);

        bool tieneRecepciones = _datos.Recepciones
            .SelectMany(r => r.Detalles)
            .Any(d => d.ArtesaniaId == id);

        if (tieneRecepciones)
        {
            throw new InvalidOperationException(
                "No se puede desactivar una artesanía que tiene movimientos de recepción."
            );
        }

        artesania.Activa = false;

        _repositorio.Guardar(_datos);
    }
}
