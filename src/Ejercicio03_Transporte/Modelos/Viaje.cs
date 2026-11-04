using System;
using System.Collections.Generic;

namespace Ejercicio03_Transporte.Modelos
{
    public enum TipoViaje
    {
        Directo = 1,
        ConParadas = 2
    }

    public class Viaje
    {
        public string IdViaje { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public DateTime FechaHoraSalida { get; set; }
        public decimal TarifaBase { get; set; }
        public TipoViaje Tipo { get; set; }
        public int CapacidadAsientos { get; set; }
        public List<int> AsientosOcupados { get; set; }

        public Viaje(string idViaje, string origen, string destino, DateTime fechaHoraSalida, decimal tarifaBase, TipoViaje tipo, int capacidadAsientos)
        {
            IdViaje = idViaje;
            Origen = origen;
            Destino = destino;
            FechaHoraSalida = fechaHoraSalida;
            TarifaBase = tarifaBase;
            Tipo = tipo;
            CapacidadAsientos = capacidadAsientos;
            AsientosOcupados = new List<int>();
        }
    }
}