
using Ejercicio05_Artesanias.Modelos;
using Ejercicio05_Artesanias.Persistencia;

namespace Ejercicio05_Artesanias.Servicios;

public class DevolucionService
{
    private readonly DatosTienda _datos;
    private readonly JsonRepository _repositorio;

    public DevolucionService(
        DatosTienda datos,
        JsonRepository repositorio)
    {
        _datos = datos;
        _repositorio = repositorio;
    }

    // Registra la devolución de uno o varios lotes.
    // La clave del diccionario es el ID del detalle de recepción.
    // El valor representa la cantidad que se desea devolver.
    public void Registrar(
        string emprendedorId,
        Dictionary<string, int> cantidadesPorLote,
        string observacion = "")
    {
        ValidarDevolucion(emprendedorId, cantidadesPorLote);

        Devolucion devolucion = new()
        {
            EmprendedorId = emprendedorId,
            Fecha = DateTime.Now,
            Observacion = observacion.Trim()
        };

        foreach (var solicitud in cantidadesPorLote)
        {
            devolucion.Detalles.Add(new DetalleDevolucion
            {
                DetalleRecepcionId = solicitud.Key,
                CantidadDevuelta = solicitud.Value
            });
        }

        // Registrar la devolución.
        _datos.Devoluciones.Add(devolucion);

        // Guardar los cambios en el archivo JSON.
        _repositorio.Guardar(_datos);
    }

    // Comprueba que la devolución cumpla las reglas del negocio.
    private void ValidarDevolucion(
        string emprendedorId,
        Dictionary<string, int> cantidadesPorLote)
    {
        if (string.IsNullOrWhiteSpace(emprendedorId))
        {
            throw new ArgumentException(
                "Debe seleccionar un emprendedor."
            );
        }

        // Comprobar que el emprendedor exista.
        // No exigimos que esté activo porque puede recibir
        // una devolución aunque ya no trabaje con la tienda.
        bool emprendedorExiste = _datos.Emprendedores
            .Any(e => e.Id == emprendedorId);

        if (!emprendedorExiste)
        {
            throw new InvalidOperationException(
                "El emprendedor no existe."
            );
        }

        if (cantidadesPorLote is null ||
            cantidadesPorLote.Count == 0)
        {
            throw new ArgumentException(
                "La devolución debe contener al menos un lote."
            );
        }

        foreach (var solicitud in cantidadesPorLote)
        {
            string detalleRecepcionId = solicitud.Key;
            int cantidadADevolver = solicitud.Value;

            if (string.IsNullOrWhiteSpace(detalleRecepcionId))
            {
                throw new ArgumentException(
                    "El identificador del lote no es válido."
                );
            }

            if (cantidadADevolver <= 0)
            {
                throw new ArgumentException(
                    "La cantidad a devolver debe ser mayor que cero."
                );
            }

            // Buscar el detalle de recepción y su recepción principal.
            var lote = _datos.Recepciones
                .SelectMany(recepcion => recepcion.Detalles
                    .Where(detalle =>
                        detalle.Id == detalleRecepcionId)
                    .Select(detalle => new
                    {
                        Recepcion = recepcion,
                        Detalle = detalle
                    }))
                .FirstOrDefault();

            if (lote is null)
            {
                throw new InvalidOperationException(
                    "El lote de recepción indicado no existe."
                );
            }

            // Verificar que el lote pertenezca al emprendedor.
            if (lote.Recepcion.EmprendedorId != emprendedorId)
            {
                throw new InvalidOperationException(
                    "No se puede devolver un lote que pertenece a otro emprendedor."
                );
            }

            // Calcular las unidades que todavía están en la tienda.
            int disponible = CalcularDisponible(
                detalleRecepcionId
            );

            if (cantidadADevolver > disponible)
            {
                throw new InvalidOperationException(
                    $"No hay suficientes unidades disponibles para devolver. " +
                    $"Disponibles: {disponible}. Solicitadas: {cantidadADevolver}."
                );
            }
        }
    }

    // Calcula las unidades disponibles de un lote.
    private int CalcularDisponible(string detalleRecepcionId)
    {
        var detalle = _datos.Recepciones
            .SelectMany(r => r.Detalles)
            .FirstOrDefault(d => d.Id == detalleRecepcionId);

        if (detalle is null)
        {
            throw new InvalidOperationException(
                "No se encontró el detalle de recepción."
            );
        }

        int recibidas = detalle.CantidadRecibida;

        int vendidas = _datos.Ventas
            .SelectMany(v => v.Detalles)
            .Where(d =>
                d.DetalleRecepcionId == detalleRecepcionId)
            .Sum(d => d.CantidadVendida);

        int devueltas = _datos.Devoluciones
            .SelectMany(d => d.Detalles)
            .Where(d =>
                d.DetalleRecepcionId == detalleRecepcionId)
            .Sum(d => d.CantidadDevuelta);

        return recibidas - vendidas - devueltas;
    }

    // Devuelve todas las devoluciones registradas.
    public List<Devolucion> Listar()
    {
        return _datos.Devoluciones
            .OrderByDescending(d => d.Fecha)
            .ToList();
    }

    // Busca una devolución por su identificador.
    public Devolucion BuscarPorId(string id)
    {
        return _datos.Devoluciones
            .FirstOrDefault(d => d.Id == id)
            ?? throw new InvalidOperationException(
                "No se encontró la devolución."
            );
    }

    // Consulta las devoluciones de un emprendedor.
    public List<Devolucion> ListarPorEmprendedor(
        string emprendedorId)
    {
        return _datos.Devoluciones
            .Where(d => d.EmprendedorId == emprendedorId)
            .OrderByDescending(d => d.Fecha)
            .ToList();
    }

    // Consulta las devoluciones realizadas dentro de un período.
    public List<Devolucion> ListarPorPeriodo(
        DateTime fechaInicio,
        DateTime fechaFin)
    {
        if (fechaInicio > fechaFin)
        {
            throw new ArgumentException(
                "La fecha inicial no puede ser posterior a la fecha final."
            );
        }

        return _datos.Devoluciones
            .Where(d =>
                d.Fecha >= fechaInicio &&
                d.Fecha <= fechaFin)
            .OrderBy(d => d.Fecha)
            .ToList();
    }
}
