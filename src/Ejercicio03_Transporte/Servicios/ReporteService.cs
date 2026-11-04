using System;
using System.Linq;
using Ejercicio03_Transporte.Modelos;

namespace Ejercicio03_Transporte.Servicios
{
    public class ReporteService
    {
        private readonly GestionTransporteService _gestionService;

        public ReporteService(GestionTransporteService gestionService)
        {
            _gestionService = gestionService;
        }

        // 1. Mapa de asientos de un viaje (con asientos ocupados y libres)
        public void GenerarMapaAsientos(string idViaje)
        {
            var viaje = _gestionService.Viajes.FirstOrDefault(v => v.IdViaje == idViaje);
            if (viaje == null)
            {
                Console.WriteLine("\n[ERROR] El viaje especificado no existe.");
                return;
            }

            Console.WriteLine($"\n========================================");
            Console.WriteLine($" MAPA DE ASIENTOS: {viaje.Origen} -> {viaje.Destino} ");
            Console.WriteLine($" ID: {viaje.IdViaje} | Salida: {viaje.FechaHoraSalida}");
            Console.WriteLine($"========================================");

            for (int i = 1; i <= viaje.CapacidadAsientos; i++)
            {
                string estado = viaje.AsientosOcupados.Contains(i) ? "[X] Ocupado" : "[  ] Libre ";
                Console.Write($"Asiento {i:D2}: {estado}   ");
                if (i % 5 == 0) Console.WriteLine();
            }
            Console.WriteLine();
        }

             // 2. Recaudación por ruta en un período
        public void GenerarRecaudacionPorRuta(DateTime fechaInicio, DateTime fechaFin)
        {
            Console.WriteLine($"\n========================================");
            Console.WriteLine($" RECAUDACIÓN POR RUTA ({fechaInicio.ToShortDateString()} - {fechaFin.ToShortDateString()})");
            Console.WriteLine($"========================================");

            var boletosValidos = _gestionService.Boletos
                .Where(b => !b.EstaAnulado && b.FechaEmision >= fechaInicio && b.FechaEmision <= fechaFin);

            var porRuta = boletosValidos
                .Select(b => new { Boleto = b, Viaje = _gestionService.Viajes.FirstOrDefault(v => v.IdViaje == b.IdViaje) })
                .Where(x => x.Viaje != null)
                .GroupBy(x => $"{x.Viaje!.Origen} -> {x.Viaje!.Destino}");

            decimal totalGeneral = 0;
            foreach (var grupo in porRuta)
            {
                decimal total = grupo.Sum(x => x.Boleto.PrecioFinal);
                totalGeneral += total;
                Console.WriteLine($"Ruta: {grupo.Key} | Total: ${total:F2} ({grupo.Count()} boletos)");
            }
            Console.WriteLine($"TOTAL RECAUDADO: ${totalGeneral:F2}");
        }

        // 3. Porcentaje de ocupación de cada viaje
        public void GenerarPorcentajeOcupacion()
        {
            Console.WriteLine($"\n========================================");
            Console.WriteLine($" PORCENTAJE DE OCUPACIÓN DE CADA VIAJE   ");
            Console.WriteLine($"========================================");

            foreach (var v in _gestionService.Viajes)
            {
                double porcentaje = v.CapacidadAsientos > 0
                    ? ((double)v.AsientosOcupados.Count / v.CapacidadAsientos) * 100
                    : 0;
                Console.WriteLine($"Viaje {v.IdViaje} [{v.Origen} -> {v.Destino}]: {v.AsientosOcupados.Count}/{v.CapacidadAsientos} asientos ocupados ({porcentaje:F1}%)");
            }
        }

        // 4. Boletos anulados en un período
        public void GenerarBoletosAnulados(DateTime fechaInicio, DateTime fechaFin)
        {
            Console.WriteLine($"\n========================================");
            Console.WriteLine($" BOLETOS ANULADOS ({fechaInicio.ToShortDateString()} - {fechaFin.ToShortDateString()})");
            Console.WriteLine($"========================================");

            var anulados = _gestionService.Boletos
                .Where(b => b.EstaAnulado && b.FechaAnulacion.HasValue && b.FechaAnulacion.Value >= fechaInicio && b.FechaAnulacion.Value <= fechaFin);

            int contador = 0;
            foreach (var b in anulados)
            {
                contador++;
                Console.WriteLine($"{contador}. Boleto ID: {b.IdBoleto} | Viaje: {b.IdViaje} | Pasajero: {b.Pasajero.Nombre} (Cédula: {b.Pasajero.Cedula}) | Asiento: {b.NumeroAsiento}");
            }

            if (contador == 0)
            {
                Console.WriteLine("No se encontraron boletos anulados en el período seleccionado.");
            }
        }

        // 5. Auditoría de los asientos libres
        public void GenerarAuditoriaAsientosLibres()
        {
            Console.WriteLine($"\n========================================");
            Console.WriteLine($" AUDITORÍA DE ASIENTOS LIBRES           ");
            Console.WriteLine($"========================================");

            foreach (var v in _gestionService.Viajes)
            {
                int asientosLibres = v.CapacidadAsientos - v.AsientosOcupados.Count;
                Console.WriteLine($"Viaje {v.IdViaje} | Ruta: {v.Origen} -> {v.Destino} | Total Asientos: {v.CapacidadAsientos} | Libres: {asientosLibres} | Ocupados: {v.AsientosOcupados.Count}");
            }
        }
    }
}