using System;

namespace Ejercicio03_Transporte.Modelos
{
    public class Boleto
    {
        public string IdBoleto { get; set; } = "";
        public string IdViaje { get; set; } = "";
        public Pasajero Pasajero { get; set; } = new Pasajero();
        public int NumeroAsiento { get; set; }
        public decimal PrecioFinal { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime? FechaAnulacion { get; set; }
        public bool EstaAnulado { get; set; } = false;

        public Boleto()
        {
        }

        public Boleto(
            string idBoleto,
            string idViaje,
            Pasajero pasajero,
            int numeroAsiento,
            decimal precioFinal)
        {
            IdBoleto = idBoleto;
            IdViaje = idViaje;
            Pasajero = pasajero;
            NumeroAsiento = numeroAsiento;
            PrecioFinal = precioFinal;
            FechaEmision = DateTime.Now;
            EstaAnulado = false;
        }
    }
}