
using Ejercicio05_Artesanias.Modelos;
using Ejercicio05_Artesanias.Persistencia;
using Ejercicio05_Artesanias.Servicios;
using Ejercicio05_Artesanias.Reportes;

using System.Globalization;

namespace Ejercicio05_Artesanias;

internal class Program
{
    private static DatosTienda _datos = null!;
    private static JsonRepository _repositorio = null!;

    private static ComunidadService _comunidadService = null!;
    private static EmprendedorService _emprendedorService = null!;
    private static ArtesaniaService _artesaniaService = null!;
    private static InventarioService _inventarioService = null!;
    private static RecepcionService _recepcionService = null!;
    private static VentaService _ventaService = null!;
    private static DevolucionService _devolucionService = null!;

    private static LiquidacionService _liquidacionService = null!;
    private static DevolucionReporteService _devolucionReporteService = null!;
    private static VentasReporteService _ventasReporteService = null!;
    private static InventarioReporteService _inventarioReporteService = null!;

    static void Main(string[] args)
    {
        InicializarServicios();

        bool continuar = true;

        while (continuar)
        {
            Console.Clear();

            Console.WriteLine("======================================");
            Console.WriteLine("       TIENDA DE ARTESANIAS");
            Console.WriteLine("======================================");
            Console.WriteLine("1.  Gestionar comunidades");
            Console.WriteLine("2.  Gestionar emprendedores");
            Console.WriteLine("3.  Gestionar artesanias");
            Console.WriteLine("4.  Registrar recepcion");
            Console.WriteLine("5.  Registrar venta");
            Console.WriteLine("6.  Registrar devolucion");
            Console.WriteLine("7.  Consultar inventario");
            Console.WriteLine("8.  Reporte de liquidaciones");
            Console.WriteLine("9.  Reporte de artesanias para devolver");
            Console.WriteLine("10. Reporte de ventas");
            Console.WriteLine("11. Auditoria de inventario");
            Console.WriteLine("0.  Salir");
            Console.WriteLine("======================================");

            int opcion = LeerEntero("Seleccione una opcion: ");

            try
            {
                switch (opcion)
                {
                    case 1:
                        MenuComunidades();
                        break;

                    case 2:
                        MenuEmprendedores();
                        break;

                    case 3:
                        MenuArtesanias();
                        break;

                    case 4:
                        RegistrarRecepcion();
                        break;

                    case 5:
                        RegistrarVenta();
                        break;

                    case 6:
                        RegistrarDevolucion();
                        break;

                    case 7:
                        ConsultarInventario();
                        break;

                    case 8:
                        ReporteLiquidaciones();
                        break;

                    case 9:
                        ReporteDevoluciones();
                        break;

                    case 10:
                        ReporteVentas();
                        break;

                    case 11:
                        AuditoriaInventario();
                        break;

                    case 0:
                        continuar = false;
                        Console.WriteLine("Aplicacion finalizada.");
                        break;

                    default:
                        Console.WriteLine("Opcion no valida.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine($"ERROR: {ex.Message}");
            }

            if (continuar)
                Pausar();
        }
    }

    // Inicializa la persistencia y todos los servicios.
    private static void InicializarServicios()
    {
        _repositorio = new JsonRepository();
        _datos = _repositorio.Cargar();

        _comunidadService = new ComunidadService(_datos, _repositorio);
        _emprendedorService = new EmprendedorService(_datos, _repositorio);
        _artesaniaService = new ArtesaniaService(_datos, _repositorio);
        _inventarioService = new InventarioService(_datos);

        _recepcionService = new RecepcionService(_datos, _repositorio);
        _ventaService = new VentaService(_datos, _repositorio);
        _devolucionService = new DevolucionService(_datos, _repositorio);

        _liquidacionService = new LiquidacionService(_datos);
        _devolucionReporteService = new DevolucionReporteService(_datos);
        _ventasReporteService = new VentasReporteService(_datos);
        _inventarioReporteService = new InventarioReporteService(_datos);
    }

    // ==========================================
    // COMUNIDADES
    // ==========================================

    private static void MenuComunidades()
    {
        Console.Clear();
        Console.WriteLine("===== GESTION DE COMUNIDADES =====");
        Console.WriteLine("1. Registrar comunidad");
        Console.WriteLine("2. Listar comunidades");
        Console.WriteLine("3. Actualizar comunidad");
        Console.WriteLine("4. Desactivar comunidad");

        int opcion = LeerEntero("Seleccione: ");

        switch (opcion)
        {
            case 1:
                string nombre = LeerTexto("Nombre: ");
                string descripcion = LeerTexto("Descripcion: ");

                _comunidadService.Registrar(nombre, descripcion);

                Console.WriteLine("Comunidad registrada correctamente.");
                break;

            case 2:
                foreach (Comunidad comunidad in _comunidadService.Listar())
                {
                    Console.WriteLine(
                        $"{comunidad.Id} | {comunidad.Nombre} | " +
                        $"{comunidad.Descripcion} | Activa: {comunidad.Activa}"
                    );
                }
                break;

            case 3:
                ListarComunidades();

                string id = LeerTexto("ID de la comunidad: ");
                string nuevoNombre = LeerTexto("Nuevo nombre: ");
                string nuevaDescripcion = LeerTexto("Nueva descripcion: ");

                _comunidadService.Actualizar(
                    id,
                    nuevoNombre,
                    nuevaDescripcion
                );

                Console.WriteLine("Comunidad actualizada.");
                break;

            case 4:
                ListarComunidades();

                string idDesactivar = LeerTexto("ID de la comunidad: ");

                _comunidadService.Desactivar(idDesactivar);

                Console.WriteLine("Comunidad desactivada.");
                break;

            default:
                Console.WriteLine("Opcion no valida.");
                break;
        }
    }

    private static void ListarComunidades()
    {
        Console.WriteLine("\nCOMUNIDADES:");

        foreach (Comunidad comunidad in _comunidadService.Listar())
        {
            Console.WriteLine(
                $"{comunidad.Id} | {comunidad.Nombre} | Activa: {comunidad.Activa}"
            );
        }
    }

    // ==========================================
    // EMPRENDEDORES
    // ==========================================

    private static void MenuEmprendedores()
    {
        Console.Clear();
        Console.WriteLine("===== GESTION DE EMPRENDEDORES =====");
        Console.WriteLine("1. Registrar emprendedor");
        Console.WriteLine("2. Listar emprendedores");
        Console.WriteLine("3. Actualizar emprendedor");
        Console.WriteLine("4. Desactivar emprendedor");

        int opcion = LeerEntero("Seleccione: ");

        switch (opcion)
        {
            case 1:
                ListarComunidades();

                string comunidadId = LeerTexto("ID de la comunidad: ");
                string nombres = LeerTexto("Nombres: ");
                string apellidos = LeerTexto("Apellidos: ");
                string cedula = LeerTexto("Cedula: ");
                string telefono = LeerTexto("Telefono: ");
                string correo = LeerTexto("Correo: ");

                _emprendedorService.Registrar(
                    comunidadId,
                    nombres,
                    apellidos,
                    cedula,
                    telefono,
                    correo
                );

                Console.WriteLine("Emprendedor registrado.");
                break;

            case 2:
                ListarEmprendedores();
                break;

            case 3:
                ListarEmprendedores();

                string id = LeerTexto("ID del emprendedor: ");
                string nuevaComunidad = LeerTexto("ID de la nueva comunidad: ");
                string nuevosNombres = LeerTexto("Nombres: ");
                string nuevosApellidos = LeerTexto("Apellidos: ");
                string nuevaCedula = LeerTexto("Cedula: ");
                string nuevoTelefono = LeerTexto("Telefono: ");
                string nuevoCorreo = LeerTexto("Correo: ");

                _emprendedorService.Actualizar(
                    id,
                    nuevaComunidad,
                    nuevosNombres,
                    nuevosApellidos,
                    nuevaCedula,
                    nuevoTelefono,
                    nuevoCorreo
                );

                Console.WriteLine("Emprendedor actualizado.");
                break;

            case 4:
                ListarEmprendedores();

                string idDesactivar = LeerTexto("ID del emprendedor: ");

                _emprendedorService.Desactivar(idDesactivar);

                Console.WriteLine("Emprendedor desactivado.");
                break;

            default:
                Console.WriteLine("Opcion no valida.");
                break;
        }
    }

    private static void ListarEmprendedores()
    {
        Console.WriteLine("\nEMPRENDEDORES:");

        foreach (Emprendedor emprendedor in _emprendedorService.Listar())
        {
            Console.WriteLine(
                $"{emprendedor.Id} | " +
                $"{emprendedor.Nombres} {emprendedor.Apellidos} | " +
                $"Cedula: {emprendedor.Cedula} | " +
                $"Activo: {emprendedor.Activo}"
            );
        }
    }

    // ==========================================
    // ARTESANIAS
    // ==========================================

    private static void MenuArtesanias()
    {
        Console.Clear();
        Console.WriteLine("===== GESTION DE ARTESANIAS =====");
        Console.WriteLine("1. Registrar artesania");
        Console.WriteLine("2. Listar artesanias");
        Console.WriteLine("3. Actualizar artesania");
        Console.WriteLine("4. Desactivar artesania");

        int opcion = LeerEntero("Seleccione: ");

        switch (opcion)
        {
            case 1:
                ListarEmprendedores();
                string emprendedorId = LeerTexto("ID del emprendedor: ");

                ListarTiposArtesania();
                string tipoId = LeerTexto("ID del tipo de artesania: ");

                string nombre = LeerTexto("Nombre: ");
                string descripcion = LeerTexto("Descripcion: ");
                decimal precio = LeerDecimal("Precio de venta: ");

                _artesaniaService.Registrar(
                    emprendedorId,
                    tipoId,
                    nombre,
                    descripcion,
                    precio
                );

                Console.WriteLine("Artesania registrada.");
                break;

            case 2:
                ListarArtesanias();
                break;

            case 3:
                ListarArtesanias();

                string id = LeerTexto("ID de la artesania: ");
                string nuevoEmprendedor = LeerTexto("ID del emprendedor: ");

                ListarTiposArtesania();
                string nuevoTipo = LeerTexto("ID del tipo: ");

                string nuevoNombre = LeerTexto("Nombre: ");
                string nuevaDescripcion = LeerTexto("Descripcion: ");
                decimal nuevoPrecio = LeerDecimal("Precio: ");

                _artesaniaService.Actualizar(
                    id,
                    nuevoEmprendedor,
                    nuevoTipo,
                    nuevoNombre,
                    nuevaDescripcion,
                    nuevoPrecio
                );

                Console.WriteLine("Artesania actualizada.");
                break;

            case 4:
                ListarArtesanias();

                string idDesactivar = LeerTexto("ID de la artesania: ");

                _artesaniaService.Desactivar(idDesactivar);

                Console.WriteLine("Artesania desactivada.");
                break;

            default:
                Console.WriteLine("Opcion no valida.");
                break;
        }
    }

    private static void ListarArtesanias()
    {
        Console.WriteLine("\nARTESANIAS:");

        foreach (Artesania artesania in _artesaniaService.Listar())
        {
            Console.WriteLine(
                $"{artesania.Id} | {artesania.Nombre} | " +
                $"Precio: {artesania.PrecioVenta:C} | " +
                $"Activa: {artesania.Activa}"
            );
        }
    }

    private static void ListarTiposArtesania()
    {
        Console.WriteLine("\nTIPOS DE ARTESANIA:");

        foreach (TipoArtesania tipo in _datos.TiposArtesanias.Where(t => t.Activo))
        {
            Console.WriteLine(
                $"{tipo.Id} | {tipo.Nombre} | " +
                $"Comision: {tipo.PorcentajeComision}%"
            );
        }
    }

    // ==========================================
    // RECEPCIONES
    // ==========================================

    private static void RegistrarRecepcion()
    {
        Console.Clear();
        Console.WriteLine("===== REGISTRAR RECEPCION =====");

        ListarEmprendedores();

        string emprendedorId = LeerTexto("ID del emprendedor: ");

        List<DetalleRecepcion> detalles = new();

        bool agregar = true;

        while (agregar)
        {
            ListarArtesanias();

            string artesaniaId = LeerTexto("ID de la artesania: ");
            int cantidad = LeerEntero("Cantidad recibida: ");
            decimal precio = LeerDecimal("Precio unitario de recepcion: ");

            detalles.Add(new DetalleRecepcion
            {
                ArtesaniaId = artesaniaId,
                CantidadRecibida = cantidad,
                PrecioUnitario = precio
            });

            agregar = LeerConfirmacion("¿Agregar otra artesania? (s/n): ");
        }

        string observacion = LeerTexto("Observacion: ");

        _recepcionService.Registrar(
            emprendedorId,
            detalles,
            observacion
        );

        Console.WriteLine("Recepcion registrada correctamente.");
    }

    // ==========================================
    // VENTAS
    // ==========================================

    private static void RegistrarVenta()
    {
        Console.Clear();
        Console.WriteLine("===== REGISTRAR VENTA =====");

        Dictionary<string, int> cantidades = new();

        bool agregar = true;

        while (agregar)
        {
            ListarArtesanias();

            string artesaniaId = LeerTexto("ID de la artesania: ");
            int cantidad = LeerEntero("Cantidad a vender: ");

            if (cantidades.ContainsKey(artesaniaId))
            {
                cantidades[artesaniaId] += cantidad;
            }
            else
            {
                cantidades.Add(artesaniaId, cantidad);
            }

            agregar = LeerConfirmacion("¿Agregar otra artesania? (s/n): ");
        }

        string observacion = LeerTexto("Observacion de la venta: ");

        _ventaService.Registrar(cantidades, observacion);

        Console.WriteLine("Venta registrada correctamente.");
    }

    // ==========================================
    // DEVOLUCIONES
    // ==========================================

    private static void RegistrarDevolucion()
    {
        Console.Clear();
        Console.WriteLine("===== REGISTRAR DEVOLUCION =====");

        ListarEmprendedores();

        string emprendedorId = LeerTexto("ID del emprendedor: ");

        var lotes = _datos.Recepciones
            .Where(r => r.EmprendedorId == emprendedorId)
            .SelectMany(r => r.Detalles.Select(d => new
            {
                Recepcion = r,
                Detalle = d
            }))
            .ToList();

        Dictionary<string, int> cantidades = new();

        foreach (var lote in lotes)
        {
            int disponible = _inventarioService
                .ObtenerCantidadDisponible(lote.Detalle.Id);

            if (disponible <= 0)
                continue;

            Artesania? artesania = _datos.Artesanias
                .FirstOrDefault(a => a.Id == lote.Detalle.ArtesaniaId);

            Console.WriteLine(
                $"Lote: {lote.Detalle.Id} | " +
                $"Artesania: {artesania?.Nombre} | " +
                $"Recepcion: {lote.Recepcion.Fecha:d} | " +
                $"Disponible: {disponible}"
            );

            if (LeerConfirmacion("¿Devolver unidades de este lote? (s/n): "))
            {
                int cantidad = LeerEntero("Cantidad a devolver: ");
                cantidades.Add(lote.Detalle.Id, cantidad);
            }
        }

        if (cantidades.Count == 0)
        {
            Console.WriteLine("No se seleccionaron unidades para devolver.");
            return;
        }

        string observacion = LeerTexto("Observacion de la devolucion: ");

        _devolucionService.Registrar(
            emprendedorId,
            cantidades,
            observacion
        );

        Console.WriteLine("Devolucion registrada correctamente.");
    }

    // ==========================================
    // CONSULTA DE INVENTARIO
    // ==========================================

    private static void ConsultarInventario()
    {
        Console.Clear();
        Console.WriteLine("===== INVENTARIO DISPONIBLE =====");

        var inventario = _inventarioReporteService.Generar();

        foreach (var item in inventario)
        {
            Console.WriteLine(
                $"{item.NombreArtesania} | " +
                $"Emprendedor: {item.NombreEmprendedor} | " +
                $"Recibidas: {item.CantidadRecibida} | " +
                $"Vendidas: {item.CantidadVendida} | " +
                $"Devueltas: {item.CantidadDevuelta} | " +
                $"Disponibles: {item.CantidadDisponible}"
            );
        }

        var resumen = _inventarioReporteService.GenerarResumen();

        Console.WriteLine("\nRESUMEN:");
        Console.WriteLine($"Total recibido: {resumen.TotalRecibido}");
        Console.WriteLine($"Total vendido: {resumen.TotalVendido}");
        Console.WriteLine($"Total devuelto: {resumen.TotalDevuelto}");
        Console.WriteLine($"Total disponible: {resumen.TotalDisponible}");
    }

    // ==========================================
    // REPORTE DE LIQUIDACIONES
    // ==========================================

    private static void ReporteLiquidaciones()
    {
        Console.Clear();
        Console.WriteLine("===== LIQUIDACION POR EMPRENDEDOR =====");

        ListarEmprendedores();

        string emprendedorId = LeerTexto("ID del emprendedor: ");

        DateTime fechaInicio = LeerFecha("Fecha inicial (dd/MM/yyyy): ");
        DateTime fechaFin = LeerFecha("Fecha final (dd/MM/yyyy): ");

        var resultado = _liquidacionService.Generar(
            emprendedorId,
            fechaInicio,
            fechaFin
        );

        Console.WriteLine("\nRESULTADO DE LIQUIDACION");
        Console.WriteLine($"Emprendedor: {resultado.NombreEmprendedor}");
        Console.WriteLine($"Unidades vendidas: {resultado.CantidadUnidadesVendidas}");
        Console.WriteLine($"Total vendido: {resultado.TotalVendido:C}");
        Console.WriteLine($"Comision de la tienda: {resultado.TotalComision:C}");
        Console.WriteLine($"Monto a pagar: {resultado.MontoAPagar:C}");
    }

    // ==========================================
    // REPORTE DE DEVOLUCIONES
    // ==========================================

    private static void ReporteDevoluciones()
    {
        Console.Clear();
        Console.WriteLine("===== ARTESANIAS PARA DEVOLVER =====");

        var resultados = _devolucionReporteService.Generar();

        if (resultados.Count == 0)
        {
            Console.WriteLine("No hay artesanias pendientes de devolucion.");
            return;
        }

        foreach (var item in resultados)
        {
            Console.WriteLine(
                $"Artesania: {item.NombreArtesania} | " +
                $"Emprendedor: {item.NombreEmprendedor} | " +
                $"Dias: {item.DiasEnTienda} | " +
                $"Disponibles: {item.CantidadDisponible} | " +
                $"Lote: {item.DetalleRecepcionId}"
            );
        }
    }

    // ==========================================
    // REPORTES DE VENTAS
    // ==========================================

    private static void ReporteVentas()
    {
        Console.Clear();
        Console.WriteLine("===== REPORTES DE VENTAS =====");

        DateTime fechaInicio = LeerFecha("Fecha inicial (dd/MM/yyyy): ");
        DateTime fechaFin = LeerFecha("Fecha final (dd/MM/yyyy): ");

        Console.WriteLine("\n1. Comunidades con mayores ventas");
        Console.WriteLine("2. Artesanias mas vendidas");

        int opcion = LeerEntero("Seleccione: ");

        if (opcion == 1)
        {
            var resultados = _ventasReporteService.GenerarPorComunidad(
                fechaInicio,
                fechaFin
            );

            foreach (var item in resultados)
            {
                Console.WriteLine(
                    $"{item.NombreComunidad} | " +
                    $"Unidades: {item.CantidadUnidadesVendidas} | " +
                    $"Total: {item.TotalVendido:C}"
                );
            }
        }
        else if (opcion == 2)
        {
            var resultados = _ventasReporteService.GenerarArtesaniasMasVendidas(
                fechaInicio,
                fechaFin
            );

            foreach (var item in resultados)
            {
                Console.WriteLine(
                    $"{item.NombreArtesania} | " +
                    $"Unidades: {item.CantidadVendida} | " +
                    $"Total: {item.TotalVendido:C}"
                );
            }
        }
        else
        {
            Console.WriteLine("Opcion no valida.");
        }
    }

    // ==========================================
    // AUDITORIA DE INVENTARIO
    // ==========================================

    private static void AuditoriaInventario()
    {
        Console.Clear();
        Console.WriteLine("===== AUDITORIA DE INVENTARIO =====");

        var inventario = _inventarioReporteService.Generar();

        bool correcto = true;

        foreach (var item in inventario)
        {
            int calculado =
                item.CantidadVendida +
                item.CantidadDevuelta +
                item.CantidadDisponible;

            bool coincide = calculado == item.CantidadRecibida;

            if (!coincide)
                correcto = false;

            Console.WriteLine(
                $"{item.NombreArtesania} | " +
                $"Recibidas: {item.CantidadRecibida} | " +
                $"Vendidas: {item.CantidadVendida} | " +
                $"Devueltas: {item.CantidadDevuelta} | " +
                $"Disponibles: {item.CantidadDisponible} | " +
                $"Estado: {(coincide ? "CORRECTO" : "INCONSISTENCIA")}"
            );
        }

        Console.WriteLine();

        Console.WriteLine(
            correcto
                ? "AUDITORIA FINALIZADA: inventario consistente."
                : "ADVERTENCIA: se encontraron inconsistencias."
        );
    }

    // ==========================================
    // METODOS AUXILIARES
    // ==========================================

    private static string LeerTexto(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);

            string? valor = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(valor))
                return valor.Trim();

            Console.WriteLine("El campo no puede estar vacio.");
        }
    }

    private static int LeerEntero(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);

            if (int.TryParse(Console.ReadLine(), out int valor))
                return valor;

            Console.WriteLine("Ingrese un numero entero valido.");
        }
    }

    private static decimal LeerDecimal(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);

            string? entrada = Console.ReadLine();

            if (decimal.TryParse(
                entrada,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out decimal valor))
            {
                return valor;
            }

            if (decimal.TryParse(
                entrada,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out valor))
            {
                return valor;
            }

            Console.WriteLine("Ingrese un valor decimal valido.");
        }
    }

    private static DateTime LeerFecha(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);

            if (DateTime.TryParseExact(
                Console.ReadLine(),
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime fecha))
            {
                return fecha;
            }

            Console.WriteLine("Formato incorrecto. Ejemplo: 04/11/2026.");
        }
    }

    private static bool LeerConfirmacion(string mensaje)
    {
        Console.Write(mensaje);

        string? respuesta = Console.ReadLine();

        return respuesta?.Trim().ToLower() == "s";
    }

    private static void Pausar()
    {
        Console.WriteLine();
        Console.WriteLine("Presione ENTER para continuar...");
        Console.ReadLine();
    }
}
