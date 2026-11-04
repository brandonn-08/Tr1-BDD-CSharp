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
                    "..", "..", "..",
                    "Datos",
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

        return datos ?? throw new InvalidDataException(
            "El archivo JSON está vacío o no contiene datos válidos."
        );
    }

    public void Guardar(DatosTienda datos)
    {
        string? directorio = Path.GetDirectoryName(_rutaArchivo);

        if (directorio is not null)
        {
            Directory.CreateDirectory(directorio);
        }

        string contenido = JsonSerializer.Serialize(
            datos,
            _opcionesJson
        );

        string archivoTemporal = _rutaArchivo + ".tmp";

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
