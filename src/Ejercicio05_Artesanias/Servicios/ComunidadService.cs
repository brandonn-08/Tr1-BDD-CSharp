using Ejercicio05_Artesanias.Modelos;
using Ejercicio05_Artesanias.Persistencia;

namespace Ejercicio05_Artesanias.Servicios;

public class ComunidadService
{
    private readonly DatosTienda _datos;
    private readonly JsonRepository _repositorio;

    public ComunidadService(DatosTienda datos, JsonRepository repositorio)
    {
        _datos = datos;
        _repositorio = repositorio;
    }

    public void Registrar(string nombre, string descripcion)
    {
        _repositorio.EjecutarCambio(_datos, () =>
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException(
                    "El nombre de la comunidad es obligatorio."
                );
            }

            bool existe = _datos.Comunidades.Any(c =>
                c.Nombre.Equals(nombre.Trim(), StringComparison.OrdinalIgnoreCase)
            );

            if (existe)
            {
                throw new InvalidOperationException(
                    "Ya existe una comunidad con ese nombre."
                );
            }

            Comunidad comunidad = new()
            {
                Nombre = nombre.Trim(),
                Descripcion = descripcion.Trim()
            };

            _datos.Comunidades.Add(comunidad);

            _repositorio.Guardar(_datos);
        });
    }

    public List<Comunidad> Listar()
    {
        return _datos.Comunidades
            .OrderBy(c => c.Nombre)
            .ToList();
    }

    public Comunidad BuscarPorId(string id)
    {
        return _datos.Comunidades.FirstOrDefault(c => c.Id == id)
            ?? throw new InvalidOperationException(
                "No se encontró la comunidad."
            );
    }

    public void Actualizar(string id, string nombre, string descripcion)
    {
        _repositorio.EjecutarCambio(_datos, () =>
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException(
                    "El nombre de la comunidad es obligatorio."
                );
            }

            Comunidad comunidad = BuscarPorId(id);

            bool nombreDuplicado = _datos.Comunidades.Any(c =>
                c.Id != id &&
                c.Nombre.Equals(nombre.Trim(), StringComparison.OrdinalIgnoreCase)
            );

            if (nombreDuplicado)
            {
                throw new InvalidOperationException(
                    "Ya existe otra comunidad con ese nombre."
                );
            }

            comunidad.Nombre = nombre.Trim();
            comunidad.Descripcion = descripcion.Trim();

            _repositorio.Guardar(_datos);
        });
    }

    public void Desactivar(string id)
    {
        _repositorio.EjecutarCambio(_datos, () =>
        {
            Comunidad comunidad = BuscarPorId(id);

            bool tieneEmprendedores = _datos.Emprendedores.Any(e =>
                e.ComunidadId == id
            );

            if (tieneEmprendedores)
            {
                throw new InvalidOperationException(
                    "No se puede desactivar una comunidad que tiene emprendedores registrados."
                );
            }

            comunidad.Activa = false;

            _repositorio.Guardar(_datos);
        });
    }
}
