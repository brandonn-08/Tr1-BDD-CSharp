using System;
using Ejercicio03_Transporte.Modelos;
using Ejercicio03_Transporte.Servicios;
using Ejercicio03_Transporte.Persistencia;

namespace Ejercicio03_Transporte.Consola
{
    class Program
    {
        static void Main(string[] args)
        {
            try { Ejecutar(args); }
            catch (Exception ex) { Console.Error.WriteLine("No se pudo continuar: " + ex.Message); Environment.ExitCode = 1; }
        }

        private static DateTime LeerFechaReporte(string mensaje)
        {
            Console.Write(mensaje + " (aaaa-mm-dd): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var fecha))
            {
                throw new ArgumentException("Fecha invalida. Use aaaa-mm-dd.");
            }
            return fecha;
        }

        static void Ejecutar(string[] args)
        {
            if (args.Length > 0) ManejadorArchivos.RutaCarpeta = Path.GetFullPath(args[0]);
            GestionTransporteService servicio = new GestionTransporteService();
            ReporteService reporteService = new ReporteService(servicio);

            // Cargar datos guardados previamente en archivos
            ManejadorArchivos.CargarDatos(servicio);

            // Si no hay viajes cargados, agregar datos de prueba iniciales
            if (servicio.Viajes.Count == 0)
            {
                servicio.Viajes.Add(new Viaje("V01", "Ibarra", "Quito", DateTime.Now.AddHours(5), 10.0m, TipoViaje.Directo, 30));
                servicio.Viajes.Add(new Viaje("V02", "Ibarra", "Tulcán", DateTime.Now.AddHours(3), 8.0m, TipoViaje.ConParadas, 25));
                servicio.Viajes.Add(new Viaje("V03", "Ibarra", "Otavalo", DateTime.Now.AddHours(6), 5.0m, TipoViaje.Directo, 20));
                ManejadorArchivos.GuardarDatos(servicio);
            }

            int opcion = 0;
            do
            {
                if (!Console.IsInputRedirected) Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("   COOPERATIVA DE TRANSPORTE DE IBARRA  ");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Ver Viajes Disponibles");
                Console.WriteLine("2. Registrar Nuevo Viaje");
                Console.WriteLine("3. Eliminar Viaje");
                Console.WriteLine("4. Vender Boleto");
                Console.WriteLine("5. Anular Boleto (Política 2 horas)");
                Console.WriteLine("6. Reportes Administrativos");
                Console.WriteLine("7. Guardar datos en Archivos");
                Console.WriteLine("8. Salir");
                Console.Write("Seleccione una opción: ");

                string? entrada = Console.ReadLine();
                if (entrada == null) break;
                var respaldo = System.Text.Json.JsonSerializer.Serialize(new ManejadorArchivos.DatosTransporte { Viajes = servicio.Viajes, Boletos = servicio.Boletos });
                try
                {
                if (int.TryParse(entrada, out opcion))
                {
                    switch (opcion)
                    {
                        case 1:
                            if (!Console.IsInputRedirected) Console.Clear();
                            Console.WriteLine("--- VIAJES DISPONIBLES ---");
                            foreach (var v in servicio.Viajes)
                            {
                                int libres = v.CapacidadAsientos - v.AsientosOcupados.Count;
                                Console.WriteLine($"ID: {v.IdViaje} | {v.Origen} -> {v.Destino} | Salida: {v.FechaHoraSalida} | Tarifa: ${v.TarifaBase} | Tipo: {v.Tipo} | Libres: {libres}/{v.CapacidadAsientos}");
                            }
                            break;

                        case 2:
                            if (!Console.IsInputRedirected) Console.Clear();
                            Console.WriteLine("--- REGISTRAR NUEVO VIAJE ---");
                            Console.Write("ID del Viaje (ej. V04): ");
                            string idV = Console.ReadLine() ?? "";
                            Console.Write("Origen: ");
                            string origen = Console.ReadLine() ?? "Ibarra";
                            Console.Write("Destino (Quito / Tulcán / Otavalo): ");
                            string destino = Console.ReadLine() ?? "";
                            Console.Write("Fecha y hora de salida (formato aaaa-mm-dd hh:mm): ");
                            if (DateTime.TryParse(Console.ReadLine(), out DateTime fechaSalida))
                            {
                                Console.Write("Tarifa Base ($): ");
                                if (decimal.TryParse(Console.ReadLine()?.Replace(',', '.'), System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign, System.Globalization.CultureInfo.InvariantCulture, out decimal tarifa))
                                {
                                    Console.WriteLine("Tipo de Viaje: [1] Directo (100%), [2] Con Paradas (85%)");
                                    if (int.TryParse(Console.ReadLine(), out int tipoInt) && (tipoInt == 1 || tipoInt == 2))
                                    {
                                        Console.Write("Capacidad de Asientos: ");
                                        if (int.TryParse(Console.ReadLine(), out int capacidad))
                                        {
                                            Viaje nuevoViaje = new Viaje(idV, origen, destino, fechaSalida, tarifa, (TipoViaje)tipoInt, capacidad);
                                            if (servicio.RegistrarViaje(nuevoViaje, out string msgViaje))
                                            {
                                                Console.WriteLine($"\n[ÉXITO] {msgViaje}");
                                                ManejadorArchivos.GuardarDatos(servicio);
                                            }
                                            else
                                            {
                                                Console.WriteLine($"\n[ERROR] {msgViaje}");
                                            }
                                        }
                                        else { Console.WriteLine("Capacidad inválida."); }
                                    }
                                    else { Console.WriteLine("Tipo de viaje inválido."); }
                                }
                                else { Console.WriteLine("Tarifa inválida."); }
                            }
                            else { Console.WriteLine("Fecha y hora inválidas."); }
                            break;

                        case 3:
                            if (!Console.IsInputRedirected) Console.Clear();
                            Console.WriteLine("--- ELIMINAR VIAJE ---");
                            Console.Write("Ingrese el ID del viaje a eliminar: ");
                            string idDel = Console.ReadLine() ?? "";
                            if (servicio.EliminarViaje(idDel, out string msgDel))
                            {
                                Console.WriteLine($"\n[ÉXITO] {msgDel}");
                                ManejadorArchivos.GuardarDatos(servicio);
                            }
                            else
                            {
                                Console.WriteLine($"\n[ERROR] {msgDel}");
                            }
                            break;

                        case 4:
                            if (!Console.IsInputRedirected) Console.Clear();
                            Console.WriteLine("--- VENTA DE BOLETO ---");
                            Console.Write("Ingrese ID del Viaje: ");
                            string idViajeVenta = Console.ReadLine() ?? "";
                            Console.Write("Cédula del Pasajero: ");
                            string ced = Console.ReadLine() ?? "";
                            Console.Write("Nombre del Pasajero: ");
                            string nom = Console.ReadLine() ?? "";

                            Console.WriteLine("Tipo de Pasajero: [0] Normal, [1] Tercera Edad (50% desc), [2] Discapacidad (50% desc)");
                            if (int.TryParse(Console.ReadLine(), out int tPas) && tPas >= 0 && tPas <= 2)
                            {
                                Console.Write("Número de Asiento: ");
                                if (int.TryParse(Console.ReadLine(), out int asiento))
                                {
                                    Pasajero pasajero = new Pasajero(ced, nom, (TipoPasajero)tPas);

                                    if (servicio.VenderBoleto(idViajeVenta, pasajero, asiento, out string msgVenta))
                                    {
                                        Console.WriteLine($"\n[ÉXITO] {msgVenta}");
                                        ManejadorArchivos.GuardarDatos(servicio);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"\n[ERROR] {msgVenta}");
                                    }
                                }
                                else { Console.WriteLine("Número de asiento inválido."); }
                            }
                            else { Console.WriteLine("Tipo de pasajero inválido."); }
                            break;

                        case 5:
                            if (!Console.IsInputRedirected) Console.Clear();
                            Console.WriteLine("--- ANULACIÓN DE BOLETO ---");
                            Console.Write("Ingrese el ID del Boleto a anular (ej. BOL-0001): ");
                            string idBol = Console.ReadLine() ?? "";

                            if (servicio.AnularBoleto(idBol, out string msgAnulacion))
                            {
                                Console.WriteLine($"\n[ÉXITO] {msgAnulacion}");
                                ManejadorArchivos.GuardarDatos(servicio);
                            }
                            else
                            {
                                Console.WriteLine($"\n[ERROR] {msgAnulacion}");
                            }
                            break;

                        case 6:
                            if (!Console.IsInputRedirected) Console.Clear();
                            Console.WriteLine("--- REPORTES ADMINISTRATIVOS ---");
                            Console.WriteLine("1. Mapa de Asientos de un Viaje");
                            Console.WriteLine("2. Recaudación por Ruta (Período)");
                            Console.WriteLine("3. Porcentaje de Ocupación de cada Viaje");
                            Console.WriteLine("4. Boletos Anulados (Período)");
                            Console.WriteLine("5. Auditoría de Asientos Libres");
                            Console.Write("Seleccione una opción de reporte: ");
                            string repOpt = Console.ReadLine() ?? "";

                            if (repOpt == "1")
                            {
                                Console.Write("Ingrese ID del viaje: ");
                                string idVMap = Console.ReadLine() ?? "";
                                reporteService.GenerarMapaAsientos(idVMap);
                            }
                            else if (repOpt == "2")
                            {
                                reporteService.GenerarRecaudacionPorRuta(LeerFechaReporte("Desde"), LeerFechaReporte("Hasta"));
                            }
                            else if (repOpt == "3")
                            {
                                reporteService.GenerarPorcentajeOcupacion();
                            }
                            else if (repOpt == "4")
                            {
                                reporteService.GenerarBoletosAnulados(LeerFechaReporte("Desde"), LeerFechaReporte("Hasta"));
                            }
                            else if (repOpt == "5")
                            {
                                reporteService.GenerarAuditoriaAsientosLibres();
                            }
                            else
                            {
                                Console.WriteLine("Opción de reporte no válida.");
                            }
                            break;

                        case 7:
                            ManejadorArchivos.GuardarDatos(servicio);
                            Console.WriteLine("\n[INFO] Datos guardados correctamente en archivos.");
                            break;

                        case 8:
                            Console.WriteLine("\nSaliendo del sistema...");
                            break;

                        default:
                            Console.WriteLine("\nOpción no válida.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("\nPor favor ingrese un número válido.");
                }

                }
                catch (Exception ex)
                {
                    var anterior = System.Text.Json.JsonSerializer.Deserialize<ManejadorArchivos.DatosTransporte>(respaldo)!;
                    servicio.Viajes = anterior.Viajes;
                    servicio.Boletos = anterior.Boletos;
                    Console.WriteLine("No se realizo la operacion: " + ex.Message);
                }

                if (opcion != 8)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    if (!Console.IsInputRedirected) Console.ReadKey();
                }

            } while (opcion != 8);
        }
    }
}
