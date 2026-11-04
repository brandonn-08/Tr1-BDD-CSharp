
using Ejercicio05_Artesanias.Modelos;
using Ejercicio05_Artesanias.Persistencia;

namespace Ejercicio05_Artesanias.Reportes;

public class ResultadoVentasComunidad
{
    public string ComunidadId { get; set; } = string.Empty;
    public string NombreComunidad { get; set; } = string.Empty;
    public int CantidadUnidadesVendidas { get; set; }
    public decimal TotalVendido { get; set; }
}

public class ResultadoArtesaniaVendida
{
    public string ArtesaniaId { get; set; } = string.Empty;
    public string NombreArtesania { get; set; } = string.Empty;
    public int CantidadVendida { get; set; }
    public decimal TotalVendido { get; set; }
}

public class VentasReporteService
{
    private readonly DatosTienda _datos;

    public VentasReporteService(DatosTienda datos)
    {
        _datos = datos;
    }

    // Reporte de comunidades con mayores ventas.
    public List<ResultadoVentasComunidad> GenerarPorComunidad(
        DateTime fechaInicio,
        DateTime fechaFin)
    {
        ValidarPeriodo(fechaInicio, fechaFin);

        var detallesVendidos = ObtenerDetallesVendidos(
            fechaInicio,
            fechaFin
        );

        var ventasPorComunidad = detallesVendidos
            .GroupBy(d => d.ComunidadId)
            .Select(grupo => new ResultadoVentasComunidad
            {
                ComunidadId = grupo.Key,
                NombreComunidad = grupo.First().NombreComunidad,
                CantidadUnidadesVendidas = grupo.Sum(d => d.Cantidad),
                TotalVendido = grupo.Sum(d => d.Total)
            })
            .OrderByDescending(r => r.TotalVendido)
            .ToList();

        return ventasPorComunidad;
    }

    // Reporte de las artesanías más vendidas.
    public List<ResultadoArtesaniaVendida> GenerarArtesaniasMasVendidas(
        DateTime fechaInicio,
        DateTime fechaFin)
    {
        ValidarPeriodo(fechaInicio, fechaFin);

        var detallesVendidos = ObtenerDetallesVendidos(
            fechaInicio,
            fechaFin
        );

        return detallesVendidos
            .GroupBy(d => d.ArtesaniaId)
            .Select(grupo => new ResultadoArtesaniaVendida
            {
                ArtesaniaId = grupo.Key,
                NombreArtesania = grupo.First().NombreArtesania,
                CantidadVendida = grupo.Sum(d => d.Cantidad),
                TotalVendido = grupo.Sum(d => d.Total)
            })
            .OrderByDescending(r => r.CantidadVendida)
            .ToList();
    }

    // Obtiene las ventas relacionadas con sus emprendedores,
    // comunidades y artesanías.
    private List<DetalleVentaReporte> ObtenerDetallesVendidos(
        DateTime fechaInicio,
        DateTime fechaFin)
    {
        var lotes = _datos.Recepciones
            .SelectMany(recepcion => recepcion.Detalles.Select(detalle => new
            {
                EmprendedorId = recepcion.EmprendedorId,
                DetalleRecepcionId = detalle.Id,
                ArtesaniaId = detalle.ArtesaniaId
            }))
            .ToList();

        var resultado = new List<DetalleVentaReporte>();

        foreach (Venta venta in _datos.Ventas.Where(v =>
            v.Fecha.Date >= fechaInicio.Date &&
            v.Fecha.Date <= fechaFin.Date))
        {
            foreach (DetalleVenta detalleVenta in venta.Detalles)
            {
                var lote = lotes.FirstOrDefault(l =>
                    l.DetalleRecepcionId == detalleVenta.DetalleRecepcionId);

                if (lote is null)
                    continue;

                Emprendedor? emprendedor = _datos.Emprendedores
                    .FirstOrDefault(e => e.Id == lote.EmprendedorId);

                if (emprendedor is null)
                    continue;

                Comunidad? comunidad = _datos.Comunidades
                    .FirstOrDefault(c => c.Id == emprendedor.ComunidadId);

                Artesania? artesania = _datos.Artesanias
                    .FirstOrDefault(a => a.Id == lote.ArtesaniaId);

                if (comunidad is null || artesania is null)
                    continue;

                resultado.Add(new DetalleVentaReporte
                {
                    ComunidadId = comunidad.Id,
                    NombreComunidad = comunidad.Nombre,
                    ArtesaniaId = artesania.Id,
                    NombreArtesania = artesania.Nombre,
                    Cantidad = detalleVenta.CantidadVendida,
                    Total = detalleVenta.TotalVendido
                });
            }
        }

        return resultado;
    }

    private static void ValidarPeriodo(
        DateTime fechaInicio,
        DateTime fechaFin)
    {
        if (fechaInicio.Date > fechaFin.Date)
        {
            throw new ArgumentException(
                "La fecha inicial no puede ser posterior a la fecha final."
            );
        }
    }

    private class DetalleVentaReporte
    {
        public string ComunidadId { get; set; } = string.Empty;
        public string NombreComunidad { get; set; } = string.Empty;
        public string ArtesaniaId { get; set; } = string.Empty;
        public string NombreArtesania { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }
}
