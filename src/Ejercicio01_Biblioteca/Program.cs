using System.Text.Json;
using Biblioteca.Modelos;

string archivo = args.Length > 0 ? args[0] : "biblioteca.json";
Datos datos = File.Exists(archivo)
    ? JsonSerializer.Deserialize<Datos>(File.ReadAllText(archivo)) ?? new Datos()
    : new Datos();

while (true)
{
    Console.WriteLine("\nBIBLIOTECA UTN");
    Console.WriteLine("1 Usuario  2 Material  3 Prestar  4 Devolver  5 Pagar multa");
    Console.WriteLine("6 Listar  7 Vencidos  8 Historial  9 Más prestados");
    Console.WriteLine("10 Multas por período  11 Auditoría  0 Salir");
    string opcion = Leer("Opción: ");
    if (opcion == "0") break;
    switch (opcion)
    {
        case "1": RegistrarUsuario(); break;
        case "2": RegistrarMaterial(); break;
        case "3": Prestar(); break;
        case "4": Devolver(); break;
        case "5": Pagar(); break;
        case "6": Listar(); break;
        case "7": Vencidos(); break;
        case "8": Historial(); break;
        case "9": MasPrestados(); break;
        case "10": MultasPeriodo(); break;
        case "11": Auditoria(); break;
        default: Console.WriteLine("Opción inválida."); break;
    }
}

string Leer(string mensaje) { Console.Write(mensaje); return Console.ReadLine() ?? ""; }
int Numero(string mensaje) => int.TryParse(Leer(mensaje), out int n) ? n : -1;
Usuario? Usuario(int id) => datos.Usuarios.Find(u => u.Id == id);
Material? Material(int id) => datos.Materiales.Find(m => m.Id == id);

void Guardar()
{
    string? carpeta = Path.GetDirectoryName(Path.GetFullPath(archivo));
    if (carpeta != null) Directory.CreateDirectory(carpeta);
    File.WriteAllText(archivo, JsonSerializer.Serialize(datos, new JsonSerializerOptions { WriteIndented = true }));
}

int Disponibles(int id)
{
    Material? material = Material(id);
    if (material == null) return 0;
    int ocupados = datos.Prestamos.Count(p => p.MaterialId == id && p.Devuelto == null);
    return material.Total - ocupados;
}

void RegistrarUsuario()
{
    string nombre = Leer("Nombre: ").Trim();
    if (nombre == "") { Console.WriteLine("Nombre obligatorio."); return; }
    int id = datos.Usuarios.Count == 0 ? 1 : datos.Usuarios.Max(u => u.Id) + 1;
    datos.Usuarios.Add(new Usuario { Id = id, Nombre = nombre });
    Guardar();
    Console.WriteLine("Usuario registrado: " + id);
}

void RegistrarMaterial()
{
    string titulo = Leer("Título: ").Trim();
    int tipo = Numero("Tipo (1 libro, 2 revista): ");
    int total = Numero("Ejemplares: ");
    if (titulo == "" || (tipo != 1 && tipo != 2) || total < 1)
    { Console.WriteLine("Datos inválidos."); return; }
    int id = datos.Materiales.Count == 0 ? 1 : datos.Materiales.Max(m => m.Id) + 1;
    datos.Materiales.Add(new Material { Id = id, Titulo = titulo, Tipo = tipo == 1 ? "Libro" : "Revista", Total = total });
    Guardar();
    Console.WriteLine("Material registrado: " + id);
}

void Prestar()
{
    Usuario? usuario = Usuario(Numero("ID usuario: "));
    Material? material = Material(Numero("ID material: "));
    if (usuario == null || material == null) { Console.WriteLine("Usuario o material inexistente."); return; }
    if (usuario.MultaPendiente > 0) { Console.WriteLine("Tiene multas pendientes."); return; }
    if (datos.Prestamos.Count(p => p.UsuarioId == usuario.Id && p.Devuelto == null) >= 3)
    { Console.WriteLine("Ya tiene 3 préstamos activos."); return; }
    if (Disponibles(material.Id) < 1) { Console.WriteLine("No hay ejemplares disponibles."); return; }
    int id = datos.Prestamos.Count == 0 ? 1 : datos.Prestamos.Max(p => p.Id) + 1;
    DateTime vence = DateTime.Today.AddDays(material.Tipo == "Libro" ? 7 : 3);
    datos.Prestamos.Add(new Prestamo { Id = id, UsuarioId = usuario.Id, MaterialId = material.Id,
        Fecha = DateTime.Today, Vence = vence });
    Guardar();
    Console.WriteLine($"Préstamo {id}. Vence: {vence:yyyy-MM-dd}");
}

void Devolver()
{
    int id = Numero("ID préstamo: ");
    Prestamo? p = datos.Prestamos.Find(x => x.Id == id);
    if (p == null) { Console.WriteLine("Préstamo inexistente."); return; }
    if (p.Devuelto != null) { Console.WriteLine("Ya fue devuelto."); return; }
    Material material = Material(p.MaterialId)!;
    int dias = Math.Max(0, (DateTime.Today - p.Vence.Date).Days);
    p.Multa = dias * (material.Tipo == "Libro" ? 0.25m : 0.50m);
    p.Devuelto = DateTime.Today;
    Usuario(p.UsuarioId)!.MultaPendiente += p.Multa;
    Guardar();
    Console.WriteLine($"Devuelto. Multa: ${p.Multa:0.00}");
}

void Pagar()
{
    Usuario? u = Usuario(Numero("ID usuario: "));
    if (u == null) { Console.WriteLine("Usuario inexistente."); return; }
    Console.WriteLine($"Debe: ${u.MultaPendiente:0.00}");
    if (!decimal.TryParse(Leer("Pago: "), out decimal pago) || pago <= 0 || pago > u.MultaPendiente)
    { Console.WriteLine("Pago inválido."); return; }
    u.MultaPendiente -= pago;
    Guardar();
    Console.WriteLine("Pago registrado.");
}

void Listar()
{
    foreach (Usuario u in datos.Usuarios)
        Console.WriteLine($"Usuario {u.Id}: {u.Nombre}, debe ${u.MultaPendiente:0.00}");
    foreach (Material m in datos.Materiales)
        Console.WriteLine($"Material {m.Id}: {m.Titulo} ({m.Tipo}), {Disponibles(m.Id)}/{m.Total} disponibles");
}

void Mostrar(Prestamo p)
{
    string estado = p.Devuelto == null ? "activo" : "devuelto";
    Console.WriteLine($"#{p.Id} {Usuario(p.UsuarioId)?.Nombre} - {Material(p.MaterialId)?.Titulo}, vence {p.Vence:yyyy-MM-dd}, {estado}, multa ${p.Multa:0.00}");
}

void Vencidos()
{
    foreach (Prestamo p in datos.Prestamos)
        if (p.Devuelto == null && p.Vence.Date < DateTime.Today) Mostrar(p);
}

void Historial()
{
    int id = Numero("ID usuario: ");
    if (Usuario(id) == null) { Console.WriteLine("Usuario inexistente."); return; }
    foreach (Prestamo p in datos.Prestamos)
        if (p.UsuarioId == id) Mostrar(p);
}

void MasPrestados()
{
    foreach (Material m in datos.Materiales.OrderByDescending(m => datos.Prestamos.Count(p => p.MaterialId == m.Id)))
        Console.WriteLine($"{m.Titulo}: {datos.Prestamos.Count(p => p.MaterialId == m.Id)} préstamos");
}

void MultasPeriodo()
{
    if (!DateTime.TryParseExact(Leer("Desde (aaaa-mm-dd): "), "yyyy-MM-dd", null, 0, out DateTime desde) ||
        !DateTime.TryParseExact(Leer("Hasta (aaaa-mm-dd): "), "yyyy-MM-dd", null, 0, out DateTime hasta) || hasta < desde)
    { Console.WriteLine("Fechas inválidas."); return; }
    decimal total = 0;
    foreach (Prestamo p in datos.Prestamos)
        if (p.Devuelto >= desde.Date && p.Devuelto < hasta.Date.AddDays(1)) total += p.Multa;
    Console.WriteLine($"Multas generadas: ${total:0.00}");
}

void Auditoria()
{
    foreach (Material m in datos.Materiales)
        Console.WriteLine($"{m.Titulo}: total {m.Total}, prestados {m.Total - Disponibles(m.Id)}, disponibles {Disponibles(m.Id)}");
}
