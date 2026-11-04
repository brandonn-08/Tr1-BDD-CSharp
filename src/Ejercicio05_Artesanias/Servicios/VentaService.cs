
using Ejercicio05_Artesanias.Modelos;
using Ejercicio05_Artesanias.Persistencia;

namespace Ejercicio05_Artesanias.Servicios;

public class VentaService
{
    private readonly DatosTienda _datos;
    private readonly JsonRepository _repositorio;

    public VentaService(
        DatosTienda datos,
        JsonRepository repositorio)
    {
        _datos = datos;
        _repositorio = repositorio;
    }

    // Registra una venta de una o varias artesanías.
    // La clave del diccionario es el ID de la artesanía.
    // El valor es la cantidad que se desea vender.
    public void Registrar(
        Dictionary<string, int> cantidadesPorArtesania,
        string observacion = "")
    {
        _repositorio.EjecutarCambio(_datos, () =>
        {
            ValidarVenta(cantidadesPorArtesania);

            Venta venta = new()
            {
                Fecha = DateTime.Now,
                Observacion = observacion.Trim()
            };

            // Procesar cada artesanía solicitada.
            foreach (var solicitud in cantidadesPorArtesania)
            {
                string artesaniaId = solicitud.Key;
                int cantidadPendiente = solicitud.Value;

                Artesania artesania = _datos.Artesanias
                    .First(a => a.Id == artesaniaId);

                TipoArtesania tipo = _datos.TiposArtesanias
                    .First(t => t.Id == artesania.TipoArtesaniaId);

                // Obtener todos los lotes disponibles de la artesanía.
                // Se ordenan por fecha para aplicar FIFO.
                var lotes = _datos.Recepciones
                    .SelectMany(recepcion => recepcion.Detalles
                        .Where(detalle =>
                            detalle.ArtesaniaId == artesaniaId)
                        .Select(detalle => new
                        {
                            Recepcion = recepcion,
                            Detalle = detalle
                        }))
                    .OrderBy(l => l.Recepcion.Fecha)
                    .ToList();

                foreach (var lote in lotes)
                {
                    if (cantidadPendiente == 0)
                        break;

                    int disponible = CalcularDisponible(
                        lote.Detalle.Id
                    );

                    if (disponible <= 0)
                        continue;

                    // Vender la cantidad disponible del lote
                    // o la cantidad que todavía se necesita.
                    int cantidadVendida = Math.Min(
                        disponible,
                        cantidadPendiente
                    );

                    DetalleVenta detalleVenta = new()
                    {
                        DetalleRecepcionId = lote.Detalle.Id,
                        CantidadVendida = cantidadVendida,
                        PrecioUnitario = artesania.PrecioVenta,
                        PorcentajeComision = tipo.PorcentajeComision
                    };

                    venta.Detalles.Add(detalleVenta);

                    cantidadPendiente -= cantidadVendida;
                }
            }

            // Agregar la venta al sistema.
            _datos.Ventas.Add(venta);

            // Guardar los cambios en JSON.
            _repositorio.Guardar(_datos);
        });
    }

    // Comprueba que la venta sea válida antes de registrarla.
    private void ValidarVenta(
        Dictionary<string, int> cantidadesPorArtesania)
    {
        if (cantidadesPorArtesania is null ||
            cantidadesPorArtesania.Count == 0)
        {
            throw new ArgumentException(
                "La venta debe contener al menos una artesanía."
            );
        }

        foreach (var solicitud in cantidadesPorArtesania)
        {
            string artesaniaId = solicitud.Key;
            int cantidad = solicitud.Value;

            if (string.IsNullOrWhiteSpace(artesaniaId))
            {
                throw new ArgumentException(
                    "El identificador de la artesanía no es válido."
                );
            }

            if (cantidad <= 0)
            {
                throw new ArgumentException(
                    "La cantidad a vender debe ser mayor que cero."
                );
            }

            // Buscar la artesanía.
            Artesania? artesania = _datos.Artesanias
                .FirstOrDefault(a => a.Id == artesaniaId);

            if (artesania is null)
            {
                throw new InvalidOperationException(
                    "La artesanía solicitada no existe."
                );
            }

            if (!artesania.Activa)
            {
                throw new InvalidOperationException(
                    $"La artesanía '{artesania.Nombre}' está inactiva."
                );
            }

            if (artesania.PrecioVenta <= 0)
            {
                throw new InvalidOperationException(
                    $"La artesanía '{artesania.Nombre}' no tiene un precio válido."
                );
            }

            // Comprobar que el tipo de artesanía exista y esté activo.
            TipoArtesania? tipo = _datos.TiposArtesanias
                .FirstOrDefault(t =>
                    t.Id == artesania.TipoArtesaniaId &&
                    t.Activo
                );

            if (tipo is null)
            {
                throw new InvalidOperationException(
                    $"El tipo de artesanía de '{artesania.Nombre}' no existe o está inactivo."
                );
            }

            if (tipo.PorcentajeComision < 0 ||
                tipo.PorcentajeComision > 100)
            {
                throw new InvalidOperationException(
                    "El porcentaje de comisión no es válido."
                );
            }

            // Comprobar que haya suficiente inventario.
            int disponible = CalcularDisponibleArtesania(
                artesaniaId
            );

            if (cantidad > disponible)
            {
                throw new InvalidOperationException(
                    $"No hay suficientes unidades de '{artesania.Nombre}'. " +
                    $"Disponibles: {disponible}. Solicitadas: {cantidad}."
                );
            }
        }
    }

    // Calcula las unidades disponibles de un lote específico.
    private int CalcularDisponible(string detalleRecepcionId)
    {
        int recibidas = _datos.Recepciones
            .SelectMany(r => r.Detalles)
            .Where(d => d.Id == detalleRecepcionId)
            .Sum(d => d.CantidadRecibida);

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

    // Calcula las unidades disponibles de todos los lotes
    // correspondientes a una artesanía.
    private int CalcularDisponibleArtesania(string artesaniaId)
    {
        return _datos.Recepciones
            .SelectMany(r => r.Detalles)
            .Where(d => d.ArtesaniaId == artesaniaId)
            .Sum(d => CalcularDisponible(d.Id));
    }

    // Devuelve todas las ventas registradas.
    public List<Venta> Listar()
    {
        return _datos.Ventas
            .OrderByDescending(v => v.Fecha)
            .ToList();
    }

    // Busca una venta por su identificador.
    public Venta BuscarPorId(string id)
    {
        return _datos.Ventas
            .FirstOrDefault(v => v.Id == id)
            ?? throw new InvalidOperationException(
                "No se encontró la venta."
            );
    }

    // Consulta las ventas realizadas dentro de un período.
    public List<Venta> ListarPorPeriodo(
        DateTime fechaInicio,
        DateTime fechaFin)
    {
        if (fechaInicio.Date > fechaFin.Date)
        {
            throw new ArgumentException(
                "La fecha inicial no puede ser posterior a la fecha final."
            );
        }

        return _datos.Ventas
            .Where(v =>
                v.Fecha.Date >= fechaInicio.Date &&
                v.Fecha.Date <= fechaFin.Date)
            .OrderBy(v => v.Fecha)
            .ToList();
    }
}
