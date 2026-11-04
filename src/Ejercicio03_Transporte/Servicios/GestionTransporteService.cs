using System;
using System.Collections.Generic;
using System.Linq;
using Ejercicio03_Transporte.Modelos;

namespace Ejercicio03_Transporte.Servicios
{
    public class GestionTransporteService
    {
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

            return precioBase;
        }

        public bool RegistrarViaje(Viaje nuevoViaje, out string mensaje)
        {
            if (Viajes.Any(v => v.IdViaje == nuevoViaje.IdViaje))
            {
                mensaje = "Ya existe un viaje registrado con ese ID.";
                return false;
            }

            if (nuevoViaje.FechaHoraSalida <= DateTime.Now)
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

            // Regla estricta del enunciado: No se elimina un viaje que tenga boletos vendidos activos.
            bool tieneBoletosActivos = Boletos.Any(b => b.IdViaje == idViaje && !b.EstaAnulado);
            if (tieneBoletosActivos)
            {
                mensaje = "No se puede eliminar el viaje porque tiene boletos vendidos activos.";
                return false;
            }

            Viajes.Remove(viaje);
            mensaje = "El viaje ha sido eliminado correctamente.";
            return true;
        }

        public bool VenderBoleto(string idViaje, Pasajero pasajero, int numeroAsiento, out string mensaje)
        {
            var viaje = Viajes.FirstOrDefault(v => v.IdViaje == idViaje);
            if (viaje == null)
            {
                mensaje = "El viaje seleccionado no existe.";
                return false;
            }

            // Regla: No se venden boletos para un viaje que ya salió
            if (DateTime.Now >= viaje.FechaHoraSalida)
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

            if (viaje.AsientosOcupados.Contains(numeroAsiento))
            {
                mensaje = $"El asiento {numeroAsiento} ya está ocupado.";
                return false;
            }

            decimal precioFinal = CalcularPrecioBoleto(viaje, pasajero.Tipo);
            string idBoleto = "BOL-" + (Boletos.Count + 1).ToString("D4");

            Boleto nuevoBoleto = new Boleto(idBoleto, idViaje, pasajero, numeroAsiento, precioFinal);
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
            if (viaje != null)
            {
                // Regla estricta: Un boleto solo se puede anular hasta 2 horas antes de la salida
                double horasRestantes = (viaje.FechaHoraSalida - DateTime.Now).TotalHours;
                if (horasRestantes < 2)
                {
                    mensaje = $"No se puede anular. Faltan menos de 2 horas para la salida ({horasRestantes:F1} horas restantes).";
                    return false;
                }

                // Liberar el asiento
                viaje.AsientosOcupados.Remove(boleto.NumeroAsiento);
            }

            boleto.EstaAnulado = true;
            boleto.FechaAnulacion = DateTime.Now;

            mensaje = $"El boleto {idBoleto} fue anulado correctamente. El asiento vuelve a estar disponible.";
            return true;
        }
    }
}