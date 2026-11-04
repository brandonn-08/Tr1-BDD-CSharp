using Ejercicio05_Artesanias.Modelos;
using Ejercicio05_Artesanias.Persistencia;

namespace Ejercicio05_Artesanias.Servicios;

public class EmprendedorService
{
    private readonly DatosTienda _datos;
    private readonly JsonRepository _repositorio;

    public EmprendedorService(DatosTienda datos, JsonRepository repositorio)
    {
        _datos = datos;
        _repositorio = repositorio;
    }

    public void Registrar(
        string comunidadId,
        string nombres,
        string apellidos,
        string cedula,
        string telefono,
        string correo)
    {
        if (string.IsNullOrWhiteSpace(nombres) ||
            string.IsNullOrWhiteSpace(apellidos) ||
            string.IsNullOrWhiteSpace(cedula))
        {
            throw new ArgumentException(
                "Nombres, apellidos y cédula son obligatorios."
            );
        }

        Comunidad? comunidad = _datos.Comunidades.FirstOrDefault(c =>
            c.Id == comunidadId && c.Activa
        );

        if (comunidad is null)
        {
            throw new InvalidOperationException(
                "La comunidad no existe o está inactiva."
            );
        }

        bool cedulaExiste = _datos.Emprendedores.Any(e =>
            e.Cedula == cedula.Trim()
        );

        if (cedulaExiste)
        {
            throw new InvalidOperationException(
                "Ya existe un emprendedor registrado con esa cédula."
            );
        }

        Emprendedor emprendedor = new()
        {
            ComunidadId = comunidadId,
            Nombres = nombres.Trim(),
            Apellidos = apellidos.Trim(),
            Cedula = cedula.Trim(),
            Telefono = telefono.Trim(),
            Correo = correo.Trim()
        };

        _datos.Emprendedores.Add(emprendedor);

        _repositorio.Guardar(_datos);
    }

    public List<Emprendedor> Listar()
    {
        return _datos.Emprendedores
            .OrderBy(e => e.Apellidos)
            .ThenBy(e => e.Nombres)
            .ToList();
    }

    public Emprendedor BuscarPorId(string id)
    {
        return _datos.Emprendedores.FirstOrDefault(e => e.Id == id)
            ?? throw new InvalidOperationException(
                "No se encontró el emprendedor."
            );
    }

    public void Actualizar(
        string id,
        string comunidadId,
        string nombres,
        string apellidos,
        string cedula,
        string telefono,
        string correo)
    {
        if (string.IsNullOrWhiteSpace(nombres) ||
            string.IsNullOrWhiteSpace(apellidos) ||
            string.IsNullOrWhiteSpace(cedula))
        {
            throw new ArgumentException(
                "Nombres, apellidos y cédula son obligatorios."
            );
        }

        Emprendedor emprendedor = BuscarPorId(id);

        bool comunidadValida = _datos.Comunidades.Any(c =>
            c.Id == comunidadId && c.Activa
        );

        if (!comunidadValida)
        {
            throw new InvalidOperationException(
                "La comunidad no existe o está inactiva."
            );
        }

        bool cedulaDuplicada = _datos.Emprendedores.Any(e =>
            e.Id != id && e.Cedula == cedula.Trim()
        );

        if (cedulaDuplicada)
        {
            throw new InvalidOperationException(
                "La cédula ya pertenece a otro emprendedor."
            );
        }

        emprendedor.ComunidadId = comunidadId;
        emprendedor.Nombres = nombres.Trim();
        emprendedor.Apellidos = apellidos.Trim();
        emprendedor.Cedula = cedula.Trim();
        emprendedor.Telefono = telefono.Trim();
        emprendedor.Correo = correo.Trim();

        _repositorio.Guardar(_datos);
    }

    public void Desactivar(string id)
    {
        Emprendedor emprendedor = BuscarPorId(id);

        bool tieneArtesanias = _datos.Artesanias.Any(a =>
            a.EmprendedorId == id
        );

        if (tieneArtesanias)
        {
            throw new InvalidOperationException(
                "No se puede desactivar un emprendedor que tiene artesanías registradas."
            );
        }

        emprendedor.Activo = false;

        _repositorio.Guardar(_datos);
    }
}
