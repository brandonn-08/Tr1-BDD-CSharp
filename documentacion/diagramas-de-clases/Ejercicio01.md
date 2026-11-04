# Diagrama de clases: biblioteca

```mermaid
classDiagram
    class Usuario {
        +int Id
        +string Nombre
        +decimal MultaPendiente
    }
    class Material {
        +int Id
        +string Titulo
        +string Tipo
        +int Total
    }
    class Prestamo {
        +int Id
        +int UsuarioId
        +int MaterialId
        +DateTime Fecha
        +DateTime Vence
        +DateTime? Devuelto
        +decimal Multa
    }
    class Datos {
        +List~Usuario~ Usuarios
        +List~Material~ Materiales
        +List~Prestamo~ Prestamos
    }
    Usuario "1" --> "0..*" Prestamo : realiza
    Material "1" --> "0..*" Prestamo : se presta
    Datos "1" o-- "0..*" Usuario
    Datos "1" o-- "0..*" Material
    Datos "1" o-- "0..*" Prestamo
```

Cada préstamo guarda los identificadores del usuario y del material. `Datos` reúne las tres listas para guardarlas en un solo archivo JSON. Los libros y las revistas se distinguen mediante el campo `Tipo`. Las operaciones del menú están en `Program.cs`.
