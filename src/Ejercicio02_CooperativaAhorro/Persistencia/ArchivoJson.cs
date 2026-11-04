using System.Text.Json;
using Ejercicio02_CooperativaAhorro.Modelos;

namespace Ejercicio02_CooperativaAhorro.Persistencia;

public class ArchivoJson
{
    public string Ruta { get; }
    private static readonly JsonSerializerOptions Opciones = new() { WriteIndented = true };
    public ArchivoJson(string ruta) => Ruta = Path.GetFullPath(ruta);
    public DatosCooperativa Cargar()
    {
        if (!File.Exists(Ruta)) return new();
        // Si el archivo esta danado se informa el error; no se reemplaza por datos vacios.
        return JsonSerializer.Deserialize<DatosCooperativa>(File.ReadAllText(Ruta), Opciones)
            ?? throw new InvalidDataException("El archivo JSON no contiene datos.");
    }
    public static DatosCooperativa Copiar(DatosCooperativa datos) =>
        JsonSerializer.Deserialize<DatosCooperativa>(JsonSerializer.Serialize(datos, Opciones), Opciones)!;
    public void Guardar(DatosCooperativa datos)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Ruta)!);
        string temporal = Ruta + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllText(temporal, JsonSerializer.Serialize(datos, Opciones));
            File.Move(temporal, Ruta, true);
        }
        finally { if (File.Exists(temporal)) File.Delete(temporal); }
    }
}
