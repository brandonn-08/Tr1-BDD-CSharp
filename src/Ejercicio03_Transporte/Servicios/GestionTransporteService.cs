using System;
using System.Collections.Generic;
using System.Linq;
using Ejercicio03_Transporte.Modelos;

namespace Ejercicio03_Transporte.Servicios
{
    public class GestionTransporteService
    {
        private readonly Func<DateTime> reloj;

        public GestionTransporteService(Func<DateTime>? reloj = null)
        {
            this.reloj = reloj ?? (() => DateTime.Now);
        }

        public List<Viaje> Viajes { get; set; } = new List<Viaje>();
        public List<Boleto> Boletos { get; set; } = new List<Boleto>();

        public decimal CalcularPrecioBoleto(Viaje viaje, TipoPasajero tipoPasajero)
        {
            // Regla: Viajes con paradas intermedias cobran el 85% de la tarifa base. Directos el 100%.
            decimal precioBase = viaje.Tipo == TipoViaje.ConParadas ? viaje.TarifaBase * 0.85m : viaje.TarifaBase;

            // Regla: Tercera edad y personas con discapacidad reciben un 50% de descuento.
            if (tipoPasajero == TipoPasajero.TerceraEdad || tipoPasajero == TipoPasajero.Discapacidad)
            {
                precioBase *= 0.50m;
            }

            return decimal.Round(precioBase, 2, MidpointRounding.AwayFromZero);
        }

        public bool RegistrarViaje(Viaje nuevoViaje, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(nuevoViaje.IdViaje) ||
                nuevoViaje.Origen != "Ibarra" ||
                !new[] { "Quito", "Tulcán", "Otavalo" }.Contains(nuevoViaje.Destino) ||
                nuevoViaje.CapacidadAsientos <= 0 || nuevoViaje.TarifaBase <= 0 ||
                !Enum.IsDefined(nuevoViaje.Tipo) || nuevoViaje.AsientosOcupados.Count != 0)
            {
                mensaje = "Revise ID, ruta, tarifa, tipo y capacidad. Un viaje nuevo no tiene asientos ocupados.";
                return false;
            }

            if (Viajes.Any(v => v.IdViaje == nuevoViaje.IdViaje))
            {
                mensaje = "Ya existe un viaje registrado con ese ID.";
                return false;
            }

            if (nuevoViaje.FechaHoraSalida <= reloj())
            {
                mensaje = "La fecha y hora de salida deben ser futuras.";
                return false;
            }

            Viajes.Add(nuevoViaje);
            mensaje = "Viaje registrado exitosamente.";
            return true;
        }

        public bool EliminarViaje(string idViaje, out string mensaje)
        {
            var viaje = Viajes.FirstOrDefault(v => v.IdViaje == idViaje);
            if (viaje == null)
            {
                mensaje = "El viaje especificado no existe.";
                return false;
            }

            // Regla estricta del enunciado: No se elimina un viaje con boletos vendidos, incluso si fueron anulados.
            bool tieneBoletosActivos = Boletos.Any(b => b.IdViaje == idViaje);
            if (tieneBoletosActivos)
            {
                mensaje = "No se puede eliminar el viaje porque tiene historial de boletos vendidos.";
                return false;
            }

            Viajes.Remove(viaje);
            mensaje = "El viaje ha sido eliminado correctamente.";
            return true;
        }

        public bool VenderBoleto(string idViaje, Pasajero pasajero, int numeroAsiento, out string mensaje)
        {
            if (pasajero == null || string.IsNullOrWhiteSpace(pasajero.Cedula) ||
                string.IsNullOrWhiteSpace(pasajero.Nombre) || !Enum.IsDefined(pasajero.Tipo))
            {
                mensaje = "Cedula, nombre y tipo de pasajero validos son obligatorios.";
                return false;
            }

            var viaje = Viajes.FirstOrDefault(v => v.IdViaje == idViaje);
            if (viaje == null)
            {
                mensaje = "El viaje seleccionado no existe.";
                return false;
            }

            // Regla: No se venden boletos para un viaje que ya salió
            if (reloj() >= viaje.FechaHoraSalida)
            {
                mensaje = "No se pueden vender boletos para un viaje que ya salió.";
                return false;
            }

            // Regla: No se vende un asiento que ya está ocupado ni fuera de rango
            if (numeroAsiento < 1 || numeroAsiento > viaje.CapacidadAsientos)
            {
                mensaje = $"El número de asiento debe estar entre 1 y {viaje.CapacidadAsientos}.";
                return false;
            }

            if (viaje.AsientosOcupados.Contains(numeroAsiento) ||
                Boletos.Any(b => b.IdViaje == idViaje && !b.EstaAnulado && b.NumeroAsiento == numeroAsiento))
            {
                mensaje = $"El asiento {numeroAsiento} ya está ocupado.";
                return false;
            }

            decimal precioFinal = CalcularPrecioBoleto(viaje, pasajero.Tipo);
            string idBoleto = "BOL-" + Guid.NewGuid().ToString("N");

            Boleto nuevoBoleto = new Boleto(idBoleto, idViaje, pasajero, numeroAsiento, precioFinal);
            nuevoBoleto.FechaEmision = reloj();
            Boletos.Add(nuevoBoleto);
            viaje.AsientosOcupados.Add(numeroAsiento);

            mensaje = $"Boleto emitido con éxito. ID: {idBoleto} | Asiento: {numeroAsiento} | Total a pagar: ${precioFinal:F2}";
            return true;
        }

        public bool AnularBoleto(string idBoleto, out string mensaje)
        {
            var boleto = Boletos.FirstOrDefault(b => b.IdBoleto == idBoleto);
            if (boleto == null)
            {
                mensaje = "El boleto ingresado no existe.";
                return false;
            }

            if (boleto.EstaAnulado)
            {
                mensaje = "Este boleto ya fue anulado anteriormente (no se anula dos veces).";
                return false;
            }

            var viaje = Viajes.FirstOrDefault(v => v.IdViaje == boleto.IdViaje);
            if (viaje == null)
            {
                mensaje = "El viaje del boleto no existe; no se puede anular.";
                return false;
            }
            if (viaje != null)
            {
                // Regla estricta: Un boleto solo se puede anular hasta 2 horas antes de la salida
                double horasRestantes = (viaje.FechaHoraSalida - reloj()).TotalHours;
                if (horasRestantes < 2)
                {
                    mensaje = $"No se puede anular. Faltan menos de 2 horas para la salida ({horasRestantes:F1} horas restantes).";
                    return false;
                }

                // Liberar el asiento
                viaje.AsientosOcupados.Remove(boleto.NumeroAsiento);
            }

            boleto.EstaAnulado = true;
            boleto.FechaAnulacion = reloj();

            mensaje = $"El boleto {idBoleto} fue anulado correctamente. El asiento vuelve a estar disponible.";
            return true;
        }
    }
}
