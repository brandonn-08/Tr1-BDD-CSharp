using System;
using System.IO;
using System.Collections.Generic;
using Ejercicio03_Transporte.Modelos;
using Ejercicio03_Transporte.Servicios;

namespace Ejercicio03_Transporte.Persistencia
{
    public static class ManejadorArchivos
    {
        private static string rutaCarpeta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datos");

        public static void GuardarDatos(GestionTransporteService servicio)
        {
            if (!Directory.Exists(rutaCarpeta))
            {
                Directory.CreateDirectory(rutaCarpeta);
            }

            string rutaViajes = Path.Combine(rutaCarpeta, "viajes.txt");
            string rutaBoletos = Path.Combine(rutaCarpeta, "boletos.txt");

            // Guardar Viajes
            using (StreamWriter writer = new StreamWriter(rutaViajes))
            {
                foreach (var v in servicio.Viajes)
                {
                    string asientos = string.Join(",", v.AsientosOcupados);
                    writer.WriteLine($"{v.IdViaje}|{v.Origen}|{v.Destino}|{v.FechaHoraSalida:o}|{v.TarifaBase}|{(int)v.Tipo}|{v.CapacidadAsientos}|{asientos}");
                }
            }

            // Guardar Boletos
            using (StreamWriter writer = new StreamWriter(rutaBoletos))
            {
                foreach (var b in servicio.Boletos)
                {
                    string fechaAnulacionStr = b.FechaAnulacion.HasValue ? b.FechaAnulacion.Value.ToString("o") : "";
                    writer.WriteLine($"{b.IdBoleto}|{b.IdViaje}|{b.Pasajero.Cedula}|{b.Pasajero.Nombre}|{(int)b.Pasajero.Tipo}|{b.NumeroAsiento}|{b.PrecioFinal}|{b.FechaEmision:o}|{b.EstaAnulado}|{fechaAnulacionStr}");
                }
            }
        }

        public static void CargarDatos(GestionTransporteService servicio)
        {
            if (!Directory.Exists(rutaCarpeta)) return;

            string rutaViajes = Path.Combine(rutaCarpeta, "viajes.txt");
            string rutaBoletos = Path.Combine(rutaCarpeta, "boletos.txt");

            // Cargar Viajes
            if (File.Exists(rutaViajes))
            {
                servicio.Viajes.Clear();
                foreach (var linea in File.ReadAllLines(rutaViajes))
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;
                    var partes = linea.Split('|');
                    if (partes.Length >= 7)
                    {
                        var viaje = new Viaje(
                            partes[0], partes[1], partes[2],
                            DateTime.Parse(partes[3]),
                            decimal.Parse(partes[4]),
                            (TipoViaje)int.Parse(partes[5]),
                            int.Parse(partes[6])
                        );

                        if (partes.Length >= 8 && !string.IsNullOrEmpty(partes[7]))
                        {
                            foreach (var aStr in partes[7].Split(','))
                            {
                                if (int.TryParse(aStr, out int asiento))
                                {
                                    viaje.AsientosOcupados.Add(asiento);
                                }
                            }
                        }
                        servicio.Viajes.Add(viaje);
                    }
                }
            }

            // Cargar Boletos
            if (File.Exists(rutaBoletos))
            {
                servicio.Boletos.Clear();
                foreach (var linea in File.ReadAllLines(rutaBoletos))
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;
                    var partes = linea.Split('|');
                    if (partes.Length >= 9)
                    {
                        var pasajero = new Pasajero(partes[2], partes[3], (TipoPasajero)int.Parse(partes[4]));
                        var boleto = new Boleto(partes[0], partes[1], pasajero, int.Parse(partes[5]), decimal.Parse(partes[6]))
                        {
                            FechaEmision = DateTime.Parse(partes[7]),
                            EstaAnulado = bool.Parse(partes[8])
                        };

                        if (partes.Length >= 10 && !string.IsNullOrEmpty(partes[9]))
                        {
                            boleto.FechaAnulacion = DateTime.Parse(partes[9]);
                        }

                        servicio.Boletos.Add(boleto);
                    }
                }
            }
        }
    }
}