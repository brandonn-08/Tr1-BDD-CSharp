
using Ejercicio05_Artesanias.Modelos;
using Ejercicio05_Artesanias.Persistencia;

namespace Ejercicio05_Artesanias.Servicios;

public class RecepcionService
{
    private readonly DatosTienda _datos;
    private readonly JsonRepository _repositorio;

    public RecepcionService(
        DatosTienda datos,
        JsonRepository repositorio)
    {
        _datos = datos;
        _repositorio = repositorio;
    }

    // Registra una nueva recepción de artesanías.
    public void Registrar(
        string emprendedorId,
        List<DetalleRecepcion> detalles,
        string observacion = "")
    {
        _repositorio.EjecutarCambio(_datos, () =>
        {
            ValidarRecepcion(emprendedorId, detalles);

            // Se crea una nueva recepción.
            Recepcion recepcion = new()
            {
                EmprendedorId = emprendedorId,
                Fecha = DateTime.Now,
                Observacion = observacion.Trim(),
                Detalles = detalles.Select(d => new DetalleRecepcion
                {
                    ArtesaniaId = d.ArtesaniaId,
                    CantidadRecibida = d.CantidadRecibida,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList()
            };

            // Se agrega la recepción a la colección.
            _datos.Recepciones.Add(recepcion);

            // Se guardan los cambios en el archivo JSON.
            _repositorio.Guardar(_datos);
        });
    }

    // Valida los datos de una recepción antes de registrarla.
    private void ValidarRecepcion(
        string emprendedorId,
        List<DetalleRecepcion> detalles)
    {
        if (string.IsNullOrWhiteSpace(emprendedorId))
        {
            throw new ArgumentException(
                "Debe seleccionar un emprendedor."
            );
        }

        // Verificar que el emprendedor exista y esté activo.
        bool emprendedorValido = _datos.Emprendedores.Any(e =>
            e.Id == emprendedorId && e.Activo
        );

        if (!emprendedorValido)
        {
            throw new InvalidOperationException(
                "El emprendedor no existe o está inactivo."
            );
        }

        // Verificar que existan detalles.
        if (detalles is null || detalles.Count == 0)
        {
            throw new ArgumentException(
                "La recepción debe contener al menos una artesanía."
            );
        }

        // Evitar registrar dos veces la misma artesanía
        // dentro de una misma recepción.
        bool artesaniasDuplicadas = detalles
            .GroupBy(d => d.ArtesaniaId)
            .Any(g => g.Count() > 1);

        if (artesaniasDuplicadas)
        {
            throw new InvalidOperationException(
                "No se puede registrar la misma artesanía dos veces en una recepción."
            );
        }

        foreach (DetalleRecepcion detalle in detalles)
        {
            if (string.IsNullOrWhiteSpace(detalle.ArtesaniaId))
            {
                throw new ArgumentException(
                    "Todos los detalles deben tener una artesanía asignada."
                );
            }

            if (detalle.CantidadRecibida <= 0)
            {
                throw new ArgumentException(
                    "La cantidad recibida debe ser mayor que cero."
                );
            }

            if (detalle.PrecioUnitario <= 0)
            {
                throw new ArgumentException(
                    "El precio unitario debe ser mayor que cero."
                );
            }

            // Buscar la artesanía y comprobar su propietario.
            Artesania? artesania = _datos.Artesanias
                .FirstOrDefault(a => a.Id == detalle.ArtesaniaId);

            if (artesania is null)
            {
                throw new InvalidOperationException(
                    "Una de las artesanías indicadas no existe."
                );
            }

            if (!artesania.Activa)
            {
                throw new InvalidOperationException(
                    $"La artesanía '{artesania.Nombre}' está inactiva."
                );
            }

            if (artesania.EmprendedorId != emprendedorId)
            {
                throw new InvalidOperationException(
                    $"La artesanía '{artesania.Nombre}' no pertenece al emprendedor seleccionado."
                );
            }
        }
    }

    // Devuelve todas las recepciones registradas.
    public List<Recepcion> Listar()
    {
        return _datos.Recepciones
            .OrderByDescending(r => r.Fecha)
            .ToList();
    }

    // Busca una recepción mediante su identificador.
    public Recepcion BuscarPorId(string id)
    {
        return _datos.Recepciones
            .FirstOrDefault(r => r.Id == id)
            ?? throw new InvalidOperationException(
                "No se encontró la recepción."
            );
    }

    // Consulta las recepciones de un emprendedor.
    public List<Recepcion> ListarPorEmprendedor(
        string emprendedorId)
    {
        return _datos.Recepciones
            .Where(r => r.EmprendedorId == emprendedorId)
            .OrderByDescending(r => r.Fecha)
            .ToList();
    }
}
