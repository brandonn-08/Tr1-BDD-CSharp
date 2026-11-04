using Ejercicio04_Cafeteria.Modelos;
using Ejercicio04_Cafeteria.Persistencia;

namespace Ejercicio04_Cafeteria.Servicios
{
    public class BebidaService
    {
        private const string NombreArchivo = "bebidas.json";
        public List<Bebida> Listar() => Archivo.Leer<Bebida>(NombreArchivo);
        public Bebida? Buscar(int id) => Listar().FirstOrDefault(x => x.Id == id);
        public Bebida Registrar(Bebida b)
        {
            Validar(b); var l = Listar(); if (l.Any(x => x.Id == b.Id)) throw new InvalidOperationException("Ya existe la bebida.");
            b.Id = b.Id <= 0 ? (l.Count == 0 ? 1 : l.Max(x => x.Id) + 1) : b.Id; l.Add(b); Archivo.Guardar(NombreArchivo, l); return b;
        }
        public void Actualizar(Bebida b) { Validar(b); var l = Listar(); var i = l.FindIndex(x => x.Id == b.Id); if (i < 0) throw new KeyNotFoundException("Bebida inexistente."); l[i] = b; Archivo.Guardar(NombreArchivo, l); }
        public void Eliminar(int id) { var l = Listar(); if (l.RemoveAll(x => x.Id == id) == 0) throw new KeyNotFoundException("Bebida inexistente."); Archivo.Guardar(NombreArchivo, l); }
        public decimal PrecioFinal(int id) => (Buscar(id) ?? throw new KeyNotFoundException("Bebida inexistente.")).Precio * 1.15m;
        private static void Validar(Bebida b) { if (string.IsNullOrWhiteSpace(b.Nombre) || b.Precio <= 0 || b.Stock < 0) throw new ArgumentException("Datos de bebida inválidos."); }
    }
}
