
using Ejercicio05_Artesanias.Modelos;
using Ejercicio05_Artesanias.Persistencia;

namespace Ejercicio05_Artesanias.Reportes;

// Contiene los resultados de la liquidación de un emprendedor.
public class ResultadoLiquidacion
{
    public string EmprendedorId { get; set; } = string.Empty;
    public string NombreEmprendedor { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }

    public int CantidadUnidadesVendidas { get; set; }

    public decimal TotalVendido { get; set; }

    public decimal TotalComision { get; set; }

    public decimal MontoAPagar { get; set; }
}

public class LiquidacionService
{
    private readonly DatosTienda _datos;

    public LiquidacionService(DatosTienda datos)
    {
        _datos = datos;
    }

    // Genera la liquidación de un emprendedor en un período.
    public ResultadoLiquidacion Generar(
        string emprendedorId,
        DateTime fechaInicio,
        DateTime fechaFin)
    {
        if (string.IsNullOrWhiteSpace(emprendedorId))
        {
            throw new ArgumentException(
                "Debe seleccionar un emprendedor."
            );
        }

        if (fechaInicio.Date > fechaFin.Date)
        {
            throw new ArgumentException(
                "La fecha inicial no puede ser posterior a la fecha final."
            );
        }

        // Buscar al emprendedor.
        Emprendedor? emprendedor = _datos.Emprendedores
            .FirstOrDefault(e => e.Id == emprendedorId);

        if (emprendedor is null)
        {
            throw new InvalidOperationException(
                "El emprendedor no existe."
            );
        }

        // Obtener las recepciones del emprendedor.
        var detallesRecepcion = _datos.Recepciones
            .Where(r => r.EmprendedorId == emprendedorId)
            .SelectMany(r => r.Detalles)
            .ToList();

        // Obtener los identificadores de los lotes
        // que pertenecen al emprendedor.
        var lotesDelEmprendedor = detallesRecepcion
            .Select(d => d.Id)
            .ToHashSet();

        // Obtener las ventas dentro del período.
        var ventasPeriodo = _datos.Ventas
            .Where(v =>
                v.Fecha.Date >= fechaInicio.Date &&
                v.Fecha.Date <= fechaFin.Date)
            .ToList();

        // Seleccionar únicamente los detalles vendidos
        // correspondientes a los lotes del emprendedor.
        var detallesVendidos = ventasPeriodo
            .SelectMany(v => v.Detalles)
            .Where(d =>
                lotesDelEmprendedor.Contains(d.DetalleRecepcionId))
            .ToList();

        // Calcular los resultados de la liquidación.
        decimal totalVendido = detallesVendidos
            .Sum(d => d.TotalVendido);

        decimal totalComision = detallesVendidos
            .Sum(d => d.ComisionTienda);

        decimal montoAPagar = detallesVendidos
            .Sum(d => d.MontoEmprendedor);

        int cantidadUnidades = detallesVendidos
            .Sum(d => d.CantidadVendida);

        return new ResultadoLiquidacion
        {
            EmprendedorId = emprendedor.Id,

            NombreEmprendedor =
                $"{emprendedor.Nombres} {emprendedor.Apellidos}",

            FechaInicio = fechaInicio.Date,
            FechaFin = fechaFin.Date,

            CantidadUnidadesVendidas = cantidadUnidades,

            TotalVendido = totalVendido,

            TotalComision = totalComision,

            MontoAPagar = montoAPagar
        };
    }

    // Genera las liquidaciones de todos los emprendedores.
    public List<ResultadoLiquidacion> GenerarTodas(
        DateTime fechaInicio,
        DateTime fechaFin)
    {
        if (fechaInicio.Date > fechaFin.Date)
        {
            throw new ArgumentException(
                "La fecha inicial no puede ser posterior a la fecha final."
            );
        }

        return _datos.Emprendedores
            .Select(e => Generar(
                e.Id,
                fechaInicio,
                fechaFin
            ))
            .OrderByDescending(r => r.MontoAPagar)
            .ToList();
    }
}
