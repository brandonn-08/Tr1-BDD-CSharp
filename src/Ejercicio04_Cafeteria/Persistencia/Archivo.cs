using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ejercicio04_Cafeteria.Persistencia
{
    public static class Archivo
    {
        private static readonly JsonSerializerOptions Opciones = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        private static string Carpeta
        {
            get
            {
                var baseDir = AppContext.BaseDirectory;
                var salida = Path.Combine(baseDir, "Persistencia", "Datos");
                Directory.CreateDirectory(salida);
                return salida;
            }
        }

        public static List<T> Leer<T>(string nombre)
        {
            var ruta = Path.Combine(Carpeta, nombre);
            if (!File.Exists(ruta) || new FileInfo(ruta).Length == 0)
            {
                Guardar(nombre, new List<T>());
                return new List<T>();
            }

            try
            {
                var texto = File.ReadAllText(ruta);
                var datos = JsonSerializer.Deserialize<List<T>>(texto, Opciones);
                return datos ?? new List<T>();
            }
            catch (JsonException)
            {
                return new List<T>();
            }
        }

        public static void Guardar<T>(string nombre, IEnumerable<T> datos)
        {
            var ruta = Path.Combine(Carpeta, nombre);
            var temporal = ruta + ".tmp";
            var texto = JsonSerializer.Serialize(datos, Opciones);
            File.WriteAllText(temporal, texto);
            File.Move(temporal, ruta, true);
        }
    }
}
