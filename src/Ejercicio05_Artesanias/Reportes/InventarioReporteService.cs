
using Ejercicio05_Artesanias.Modelos;
using Ejercicio05_Artesanias.Persistencia;

namespace Ejercicio05_Artesanias.Reportes;

public class ResultadoInventario
{
    public string DetalleRecepcionId { get; set; } = string.Empty;
    public string ArtesaniaId { get; set; } = string.Empty;
    public string NombreArtesania { get; set; } = string.Empty;
    public string EmprendedorId { get; set; } = string.Empty;
    public string NombreEmprendedor { get; set; } = string.Empty;
    public DateTime FechaRecepcion { get; set; }

    public int CantidadRecibida { get; set; }
    public int CantidadVendida { get; set; }
    public int CantidadDevuelta { get; set; }
    public int CantidadDisponible { get; set; }
}

public class InventarioReporteService
{
    private readonly DatosTienda _datos;

    public InventarioReporteService(DatosTienda datos)
    {
        _datos = datos;
    }

    // Genera el reporte detallado de inventario por lote.
    public List<ResultadoInventario> Generar()
    {
        var resultado = new List<ResultadoInventario>();

        foreach (Recepcion recepcion in _datos.Recepciones)
        {
            foreach (DetalleRecepcion detalle in recepcion.Detalles)
            {
                Artesania? artesania = _datos.Artesanias
                    .FirstOrDefault(a => a.Id == detalle.ArtesaniaId);

                Emprendedor? emprendedor = _datos.Emprendedores
                    .FirstOrDefault(e => e.Id == recepcion.EmprendedorId);

                if (artesania is null || emprendedor is null)
                    continue;

                int vendidas = _datos.Ventas
                    .SelectMany(v => v.Detalles)
                    .Where(v =>
                        v.DetalleRecepcionId == detalle.Id)
                    .Sum(v => v.CantidadVendida);

                int devueltas = _datos.Devoluciones
                    .SelectMany(d => d.Detalles)
                    .Where(d =>
                        d.DetalleRecepcionId == detalle.Id)
                    .Sum(d => d.CantidadDevuelta);

                int disponibles =
                    detalle.CantidadRecibida - vendidas - devueltas;

                resultado.Add(new ResultadoInventario
                {
                    DetalleRecepcionId = detalle.Id,
                    ArtesaniaId = artesania.Id,
                    NombreArtesania = artesania.Nombre,
                    EmprendedorId = emprendedor.Id,
                    NombreEmprendedor =
                        $"{emprendedor.Nombres} {emprendedor.Apellidos}",
                    FechaRecepcion = recepcion.Fecha,
                    CantidadRecibida = detalle.CantidadRecibida,
                    CantidadVendida = vendidas,
                    CantidadDevuelta = devueltas,
                    CantidadDisponible = disponibles
                });
            }
        }

        return resultado
            .OrderBy(r => r.NombreArtesania)
            .ThenBy(r => r.FechaRecepcion)
            .ToList();
    }

    // Consulta el inventario de una artesanía específica.
    public List<ResultadoInventario> GenerarPorArtesania(
        string artesaniaId)
    {
        if (string.IsNullOrWhiteSpace(artesaniaId))
        {
            throw new ArgumentException(
                "Debe indicar una artesanía."
            );
        }

        return Generar()
            .Where(r => r.ArtesaniaId == artesaniaId)
            .ToList();
    }

    // Genera un resumen general del inventario.
    public ResumenInventario GenerarResumen()
    {
        var inventario = Generar();

        return new ResumenInventario
        {
            TotalRecibido = inventario.Sum(r => r.CantidadRecibida),
            TotalVendido = inventario.Sum(r => r.CantidadVendida),
            TotalDevuelto = inventario.Sum(r => r.CantidadDevuelta),
            TotalDisponible = inventario.Sum(r => r.CantidadDisponible)
        };
    }
}

// Contiene los totales generales del inventario.
public class ResumenInventario
{
    public int TotalRecibido { get; set; }
    public int TotalVendido { get; set; }
    public int TotalDevuelto { get; set; }
    public int TotalDisponible { get; set; }
}
