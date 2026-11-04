using System.Text.Json;
using Ejercicio05_Artesanias.Modelos;

namespace Ejercicio05_Artesanias.Persistencia;

public class JsonRepository
{
    private readonly string _rutaArchivo;

    private readonly JsonSerializerOptions _opcionesJson = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonRepository(string? rutaArchivo = null)
    {
        _rutaArchivo = rutaArchivo ??
            Path.GetFullPath(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "datos",
                    "tienda.json"
                )
            );
    }

    public DatosTienda Cargar()
    {
        if (!File.Exists(_rutaArchivo))
        {
            DatosTienda datosIniciales = CrearDatosIniciales();

            Guardar(datosIniciales);

            return datosIniciales;
        }

        string contenido = File.ReadAllText(_rutaArchivo);

        DatosTienda? datos =
            JsonSerializer.Deserialize<DatosTienda>(
                contenido,
                _opcionesJson
            );

        datos = datos ?? throw new InvalidDataException(
            "El archivo JSON está vacío o no contiene datos válidos."
        );
        ValidadorTienda.Validar(datos);
        return datos;
    }

    public void Guardar(DatosTienda datos)
    {
        ValidadorTienda.Validar(datos);
        string? directorio = Path.GetDirectoryName(_rutaArchivo);

        if (directorio is not null)
        {
            Directory.CreateDirectory(directorio);
        }

        string contenido = JsonSerializer.Serialize(
            datos,
            _opcionesJson
        );

        string archivoTemporal = _rutaArchivo + "." + Guid.NewGuid().ToString("N") + ".tmp";

        try
        {
            File.WriteAllText(archivoTemporal, contenido);

            File.Move(
                archivoTemporal,
                _rutaArchivo,
                overwrite: true
            );
        }
        finally
        {
            if (File.Exists(archivoTemporal))
            {
                File.Delete(archivoTemporal);
            }
        }
    }

    // Conserva el mismo contenedor usado por todos los servicios si la operacion falla.
    public void EjecutarCambio(DatosTienda datos, Action cambio)
    {
        string respaldo = JsonSerializer.Serialize(datos, _opcionesJson);
        try
        {
            cambio();
        }
        catch
        {
            DatosTienda anterior = JsonSerializer.Deserialize<DatosTienda>(respaldo, _opcionesJson)!;
            datos.Comunidades = anterior.Comunidades;
            datos.Emprendedores = anterior.Emprendedores;
            datos.TiposArtesanias = anterior.TiposArtesanias;
            datos.Artesanias = anterior.Artesanias;
            datos.Recepciones = anterior.Recepciones;
            datos.Ventas = anterior.Ventas;
            datos.Devoluciones = anterior.Devoluciones;
            throw;
        }
    }

    private static DatosTienda CrearDatosIniciales()
    {
        return new DatosTienda
        {
            TiposArtesanias =
            [
                new TipoArtesania
                {
                    Nombre = "Textiles",
                    PorcentajeComision = 20m,
                    Descripcion = "Artesanías elaboradas con tejidos."
                },
                new TipoArtesania
                {
                    Nombre = "Tallas en madera",
                    PorcentajeComision = 25m,
                    Descripcion = "Artesanías elaboradas mediante tallado de madera."
                }
            ]
        };
    }
}
