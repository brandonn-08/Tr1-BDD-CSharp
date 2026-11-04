using System.Globalization;
using System.Text.Json;
using Ejercicio03_Transporte.Modelos;
using Ejercicio03_Transporte.Servicios;

namespace Ejercicio03_Transporte.Persistencia
{
    public static class ManejadorArchivos
    {
        public static string RutaCarpeta { get; set; } = Path.Combine(AppContext.BaseDirectory, "datos");

        public static void GuardarDatos(GestionTransporteService servicio)
        {
            Validar(servicio);
            Directory.CreateDirectory(RutaCarpeta);
            string ruta = Path.Combine(RutaCarpeta, "transporte.json");
            string temporal = ruta + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                // Un solo archivo evita guardar viajes nuevos con boletos antiguos.
                string contenido = JsonSerializer.Serialize(new DatosTransporte
                {
                    Viajes = servicio.Viajes,
                    Boletos = servicio.Boletos
                }, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(temporal, contenido);
                File.Move(temporal, ruta, true);
            }
            finally
            {
                if (File.Exists(temporal)) File.Delete(temporal);
            }
        }

        public static void CargarDatos(GestionTransporteService servicio)
        {
            string ruta = Path.Combine(RutaCarpeta, "transporte.json");
            var copia = new GestionTransporteService();
            if (File.Exists(ruta))
            {
                var datos = JsonSerializer.Deserialize<DatosTransporte>(File.ReadAllText(ruta))
                    ?? throw new InvalidDataException("El archivo de transporte no contiene datos.");
                copia.Viajes = datos.Viajes;
                copia.Boletos = datos.Boletos;
            }
            else
            {
                // Importar los TXT de versiones anteriores sin modificarlos.
                string viajes = Path.Combine(RutaCarpeta, "viajes.txt");
                string boletos = Path.Combine(RutaCarpeta, "boletos.txt");
                if (File.Exists(viajes))
                {
                    foreach (string linea in File.ReadLines(viajes).Where(l => !string.IsNullOrWhiteSpace(l)))
                    {
                        string[] p = linea.Split('|');
                        if (p.Length != 8) throw new InvalidDataException("Fila de viaje incompleta.");
                        var viaje = new Viaje(p[0], p[1], p[2], DateTime.Parse(p[3], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                            LeerDecimal(p[4]), (TipoViaje)int.Parse(p[5]), int.Parse(p[6]));
                        if (p[7] != "") viaje.AsientosOcupados = p[7].Split(',').Select(int.Parse).ToList();
                        copia.Viajes.Add(viaje);
                    }
                }
                if (File.Exists(boletos))
                {
                    foreach (string linea in File.ReadLines(boletos).Where(l => !string.IsNullOrWhiteSpace(l)))
                    {
                        string[] p = linea.Split('|');
                        if (p.Length != 10) throw new InvalidDataException("Fila de boleto incompleta.");
                        copia.Boletos.Add(new Boleto(p[0], p[1], new Pasajero(p[2], p[3], (TipoPasajero)int.Parse(p[4])),
                            int.Parse(p[5]), LeerDecimal(p[6]))
                        {
                            FechaEmision = DateTime.Parse(p[7], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                            EstaAnulado = bool.Parse(p[8]),
                            FechaAnulacion = p[9] == "" ? null : DateTime.Parse(p[9], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
                        });
                    }
                }
            }
            Validar(copia);
            servicio.Viajes = copia.Viajes;
            servicio.Boletos = copia.Boletos;
        }

        private static decimal LeerDecimal(string texto)
        {
            return decimal.Parse(texto.Replace(',', '.'), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
        }

        public static void Validar(GestionTransporteService servicio)
        {
            if (servicio.Viajes == null || servicio.Boletos == null ||
                servicio.Viajes.Any(v => v == null || string.IsNullOrWhiteSpace(v.IdViaje) || v.CapacidadAsientos <= 0 ||
                    v.TarifaBase <= 0 || !Enum.IsDefined(v.Tipo) || v.Origen != "Ibarra" ||
                    !new[] { "Quito", "Tulcán", "Otavalo" }.Contains(v.Destino) || v.AsientosOcupados == null) ||
                servicio.Viajes.Select(v => v.IdViaje).Distinct().Count() != servicio.Viajes.Count ||
                servicio.Boletos.Select(b => b.IdBoleto).Distinct().Count() != servicio.Boletos.Count)
                throw new InvalidDataException("Viajes o identificadores invalidos.");

            foreach (var boleto in servicio.Boletos)
            {
                var viaje = servicio.Viajes.FirstOrDefault(v => v.IdViaje == boleto.IdViaje);
                if (viaje == null || string.IsNullOrWhiteSpace(boleto.IdBoleto) || boleto.Pasajero == null ||
                    string.IsNullOrWhiteSpace(boleto.Pasajero.Cedula) || string.IsNullOrWhiteSpace(boleto.Pasajero.Nombre) ||
                    !Enum.IsDefined(boleto.Pasajero.Tipo) || boleto.NumeroAsiento < 1 || boleto.NumeroAsiento > viaje.CapacidadAsientos ||
                    boleto.PrecioFinal < 0 || boleto.FechaEmision >= viaje.FechaHoraSalida ||
                    boleto.EstaAnulado != boleto.FechaAnulacion.HasValue ||
                    (boleto.FechaAnulacion.HasValue && (boleto.FechaAnulacion < boleto.FechaEmision ||
                        boleto.FechaAnulacion > viaje.FechaHoraSalida.AddHours(-2))))
                    throw new InvalidDataException("Boleto o referencia de viaje invalida.");
            }
            foreach (var viaje in servicio.Viajes)
            {
                var vendidos = servicio.Boletos.Where(b => b.IdViaje == viaje.IdViaje && !b.EstaAnulado)
                    .Select(b => b.NumeroAsiento).ToList();
                if (vendidos.Count != vendidos.Distinct().Count() ||
                    viaje.AsientosOcupados.Count != vendidos.Count ||
                    !viaje.AsientosOcupados.Order().SequenceEqual(vendidos.Order()))
                    throw new InvalidDataException("Los asientos guardados no coinciden con los boletos activos.");
            }
        }

        public class DatosTransporte
        {
            public List<Viaje> Viajes { get; set; } = new();
            public List<Boleto> Boletos { get; set; } = new();
        }
    }
}
