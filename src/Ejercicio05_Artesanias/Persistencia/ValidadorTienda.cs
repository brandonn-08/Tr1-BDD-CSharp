using Ejercicio05_Artesanias.Modelos;

namespace Ejercicio05_Artesanias.Persistencia;

public static class ValidadorTienda
{
    public static List<string> Auditar(DatosTienda datos)
    {
        List<string> errores = new();
        void Identificadores(IEnumerable<string> ids, string entidad)
        {
            var lista = ids.ToList();
            if (lista.Any(string.IsNullOrWhiteSpace) || lista.Distinct().Count() != lista.Count)
                errores.Add("Identificadores vacios o repetidos: " + entidad);
        }
        Identificadores(datos.Comunidades.Select(x => x.Id), "comunidades");
        Identificadores(datos.Emprendedores.Select(x => x.Id), "emprendedores");
        Identificadores(datos.TiposArtesanias.Select(x => x.Id), "tipos");
        Identificadores(datos.Artesanias.Select(x => x.Id), "artesanias");
        Identificadores(datos.Recepciones.Select(x => x.Id), "recepciones");
        Identificadores(datos.Ventas.Select(x => x.Id), "ventas");
        Identificadores(datos.Devoluciones.Select(x => x.Id), "devoluciones");
        var lotes = datos.Recepciones.SelectMany(r => r.Detalles).ToList();
        Identificadores(lotes.Select(x => x.Id), "lotes");
        Identificadores(datos.Ventas.SelectMany(v => v.Detalles).Select(x => x.Id), "detalles de venta");
        Identificadores(datos.Devoluciones.SelectMany(v => v.Detalles).Select(x => x.Id), "detalles de devolucion");
        foreach (var e in datos.Emprendedores)
            if (!datos.Comunidades.Any(c => c.Id == e.ComunidadId)) errores.Add("Comunidad inexistente: " + e.Id);
        foreach (var a in datos.Artesanias)
            if (a.PrecioVenta <= 0 || !datos.Emprendedores.Any(e => e.Id == a.EmprendedorId) ||
                !datos.TiposArtesanias.Any(t => t.Id == a.TipoArtesaniaId)) errores.Add("Artesania invalida: " + a.Id);
        foreach (var tipo in datos.TiposArtesanias)
        {
            decimal esperado = tipo.Nombre.Trim().ToLowerInvariant() switch
            {
                "textiles" => 20m,
                "tallas en madera" => 25m,
                _ => -1m
            };
            if (tipo.PorcentajeComision != esperado) errores.Add("Tipo o comision invalida: " + tipo.Nombre);
        }
        foreach (var r in datos.Recepciones)
        {
            if (r.Detalles.Count == 0 || !datos.Emprendedores.Any(e => e.Id == r.EmprendedorId))
                errores.Add("Recepcion sin propietario o detalles: " + r.Id);
            foreach (var l in r.Detalles)
            {
                var a = datos.Artesanias.FirstOrDefault(a => a.Id == l.ArtesaniaId);
                if (a == null || a.EmprendedorId != r.EmprendedorId || l.CantidadRecibida <= 0 || l.PrecioUnitario <= 0)
                    errores.Add("Lote invalido: " + l.Id);
                long vendido = datos.Ventas.SelectMany(v => v.Detalles).Where(v => v.DetalleRecepcionId == l.Id).Sum(v => (long)v.CantidadVendida);
                long devuelto = datos.Devoluciones.SelectMany(v => v.Detalles).Where(v => v.DetalleRecepcionId == l.Id).Sum(v => (long)v.CantidadDevuelta);
                if (vendido + devuelto > l.CantidadRecibida) errores.Add("Stock negativo: " + l.Id);
            }
        }
        foreach (var v in datos.Ventas)
        {
            if (v.Detalles.Count == 0) errores.Add("Venta vacia: " + v.Id);
            foreach (var d in v.Detalles)
            {
                var r = datos.Recepciones.FirstOrDefault(r => r.Detalles.Any(l => l.Id == d.DetalleRecepcionId));
                if (r == null || d.CantidadVendida <= 0 || d.PrecioUnitario <= 0 ||
                    (d.PorcentajeComision != 20m && d.PorcentajeComision != 25m) || r.Fecha > v.Fecha)
                    errores.Add("Venta invalida: " + d.Id);
            }
        }
        foreach (var devolucion in datos.Devoluciones)
        {
            if (devolucion.Detalles.Count == 0) errores.Add("Devolucion vacia: " + devolucion.Id);
            foreach (var d in devolucion.Detalles)
            {
                var r = datos.Recepciones.FirstOrDefault(r => r.Detalles.Any(l => l.Id == d.DetalleRecepcionId));
                if (r == null || r.EmprendedorId != devolucion.EmprendedorId || d.CantidadDevuelta <= 0 || r.Fecha > devolucion.Fecha)
                    errores.Add("Devolucion invalida: " + d.Id);
            }
        }
        return errores;
    }

    public static void Validar(DatosTienda datos)
    {
        var errores = Auditar(datos);
        if (errores.Count > 0) throw new InvalidDataException(string.Join(Environment.NewLine, errores));
    }
}
