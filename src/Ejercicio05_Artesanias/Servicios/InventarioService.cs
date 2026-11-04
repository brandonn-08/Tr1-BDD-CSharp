using Ejercicio05_Artesanias.Persistencia;
using Ejercicio05_Artesanias.Modelos;

namespace Ejercicio05_Artesanias.Servicios;

public class InventarioService
{
    private readonly DatosTienda _datos;

    public InventarioService(DatosTienda datos)
    {
        _datos = datos;
    }

    public int ObtenerUnidadesVendidas(string detalleRecepcionId)
    {
        return _datos.Ventas
            .SelectMany(venta => venta.Detalles)
            .Where(detalle =>
                detalle.DetalleRecepcionId == detalleRecepcionId)
            .Sum(detalle => detalle.CantidadVendida);
    }

    public int ObtenerUnidadesDevueltas(string detalleRecepcionId)
    {
        return _datos.Devoluciones
            .SelectMany(devolucion => devolucion.Detalles)
            .Where(detalle =>
                detalle.DetalleRecepcionId == detalleRecepcionId)
            .Sum(detalle => detalle.CantidadDevuelta);
    }

    public int ObtenerCantidadDisponible(string detalleRecepcionId)
    {
        DetalleRecepcion? detalle = _datos.Recepciones
            .SelectMany(recepcion => recepcion.Detalles)
            .FirstOrDefault(detalle =>
                detalle.Id == detalleRecepcionId);

        if (detalle is null)
        {
            throw new InvalidOperationException(
                "No se encontró el lote de recepción."
            );
        }

        int vendidas = ObtenerUnidadesVendidas(detalleRecepcionId);
        int devueltas = ObtenerUnidadesDevueltas(detalleRecepcionId);

        return detalle.CantidadRecibida - vendidas - devueltas;
    }

    public List<(Recepcion Recepcion, DetalleRecepcion Detalle, int Disponible)>
        ObtenerLotesDisponibles(string artesaniaId)
    {
        var lotes = new List<
            (Recepcion Recepcion, DetalleRecepcion Detalle, int Disponible)
        >();

        foreach (Recepcion recepcion in _datos.Recepciones)
        {
            foreach (DetalleRecepcion detalle in recepcion.Detalles)
            {
                Artesania? artesania = _datos.Artesanias
                    .FirstOrDefault(a => a.Id == detalle.ArtesaniaId);

                if (artesania is null || artesania.Id != artesaniaId)
                {
                    continue;
                }

                int disponible =
                    ObtenerCantidadDisponible(detalle.Id);

                if (disponible > 0)
                {
                    lotes.Add((recepcion, detalle, disponible));
                }
            }
        }

        return lotes;
    }

    public List<(Recepcion Recepcion, DetalleRecepcion Detalle,
                 Artesania Artesania, int Disponible)>
        ObtenerArtesaniasParaDevolver(DateTime? fechaCorte = null)
    {
        DateTime fechaActual = fechaCorte ?? DateTime.Now;

        var resultados = new List<
            (Recepcion Recepcion, DetalleRecepcion Detalle,
             Artesania Artesania, int Disponible)
        >();

        foreach (Recepcion recepcion in _datos.Recepciones)
        {
            foreach (DetalleRecepcion detalle in recepcion.Detalles)
            {
                int disponible =
                    ObtenerCantidadDisponible(detalle.Id);

                bool superaNoventaDias =
                    recepcion.Fecha.AddDays(90) < fechaActual;

                if (disponible <= 0 || !superaNoventaDias)
                {
                    continue;
                }

                Artesania? artesania = _datos.Artesanias
                    .FirstOrDefault(a => a.Id == detalle.ArtesaniaId);

                if (artesania is null)
                {
                    continue;
                }

                resultados.Add((
                    recepcion,
                    detalle,
                    artesania,
                    disponible
                ));
            }
        }

        return resultados;
    }
}
