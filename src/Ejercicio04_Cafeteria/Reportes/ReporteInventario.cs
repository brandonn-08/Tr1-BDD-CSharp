using Ejercicio04_Cafeteria.Persistencia;

namespace Ejercicio04_Cafeteria.Reportes
{
    public class ReporteInventario
    {
        public void Mostrar() { var ins = Archivo.Leer<Modelos.Ingrediente>("ingredientes.json").ToDictionary(x => x.Id); foreach (var m in Archivo.Leer<Modelos.Movimiento>("movimientos.json").OrderBy(x => x.Fecha)) Console.WriteLine($"{m.Fecha:g} | {ins.GetValueOrDefault(m.IngredienteId)?.Nombre ?? "?"} | {m.Tipo} | {m.Cantidad:F2} | {m.StockAnterior:F2} -> {m.StockNuevo:F2} | {m.Descripcion}"); }
    }
}
