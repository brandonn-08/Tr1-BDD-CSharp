
using Ejercicio05_Artesanias.Modelos;
using Ejercicio05_Artesanias.Persistencia;

namespace Ejercicio05_Artesanias.Reportes;

public class ArtesaniaParaDevolver
{
    public string EmprendedorId { get; set; } = string.Empty;
    public string NombreEmprendedor { get; set; } = string.Empty;
    public string ArtesaniaId { get; set; } = string.Empty;
    public string NombreArtesania { get; set; } = string.Empty;
    public string DetalleRecepcionId { get; set; } = string.Empty;
    public DateTime FechaRecepcion { get; set; }
    public int CantidadRecibida { get; set; }
    public int CantidadVendida { get; set; }
    public int CantidadDevuelta { get; set; }
    public int CantidadDisponible { get; set; }
    public int DiasEnTienda { get; set; }
}

public class DevolucionReporteService
{
    private readonly DatosTienda _datos;

    public DevolucionReporteService(DatosTienda datos)
    {
        _datos = datos;
    }

    // Obtiene los lotes que superan los 90 días
    // y todavía tienen unidades disponibles.
    public List<ArtesaniaParaDevolver> Generar(
        DateTime? fechaActual = null)
    {
        DateTime fechaConsulta = fechaActual ?? DateTime.Now;

        var resultado = new List<ArtesaniaParaDevolver>();

        foreach (Recepcion recepcion in _datos.Recepciones)
        {
            foreach (DetalleRecepcion detalle in recepcion.Detalles)
            {
                int diasEnTienda =
                    (fechaConsulta.Date - recepcion.Fecha.Date).Days;

                if (recepcion.Fecha.AddDays(90) >= fechaConsulta)
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

                if (disponibles <= 0)
                    continue;

                Emprendedor? emprendedor = _datos.Emprendedores
                    .FirstOrDefault(e =>
                        e.Id == recepcion.EmprendedorId);

                Artesania? artesania = _datos.Artesanias
                    .FirstOrDefault(a =>
                        a.Id == detalle.ArtesaniaId);

                if (emprendedor is null || artesania is null)
                    continue;

                resultado.Add(new ArtesaniaParaDevolver
                {
                    EmprendedorId = emprendedor.Id,

                    NombreEmprendedor =
                        $"{emprendedor.Nombres} {emprendedor.Apellidos}",

                    ArtesaniaId = artesania.Id,
                    NombreArtesania = artesania.Nombre,

                    DetalleRecepcionId = detalle.Id,

                    FechaRecepcion = recepcion.Fecha,

                    CantidadRecibida = detalle.CantidadRecibida,

                    CantidadVendida = vendidas,

                    CantidadDevuelta = devueltas,

                    CantidadDisponible = disponibles,

                    DiasEnTienda = diasEnTienda
                });
            }
        }

        return resultado
            .OrderByDescending(a => a.DiasEnTienda)
            .ToList();
    }

    // Consulta las artesanías pendientes de devolución
    // de un emprendedor específico.
    public List<ArtesaniaParaDevolver> GenerarPorEmprendedor(
        string emprendedorId,
        DateTime? fechaActual = null)
    {
        return Generar(fechaActual)
            .Where(a => a.EmprendedorId == emprendedorId)
            .ToList();
    }
}
