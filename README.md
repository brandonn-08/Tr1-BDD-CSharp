# Trabajo 1 - Bases de Datos

Una solucion con cinco proyectos de consola C# independientes. Solo se ha preparado la estructura; los ejercicios, la persistencia, los reportes y las pruebas estan pendientes de implementar por el grupo.

## Requisitos

- SDK de .NET 10.
- Para trabajar desde Visual Studio, una version compatible con .NET 10 y la carga de trabajo de desarrollo de escritorio de .NET.

## Compilar

Desde la carpeta raiz del repositorio:

```powershell
dotnet build Tr1-BDD-CSharp.sln
```

## Ejecutar un ejercicio

```powershell
dotnet run --project src/Ejercicio01_Biblioteca
dotnet run --project src/Ejercicio02_CooperativaAhorro
dotnet run --project src/Ejercicio03_Transporte
dotnet run --project src/Ejercicio04_Cafeteria
dotnet run --project src/Ejercicio05_Artesanias
```

Cada comando ejecuta un proyecto distinto. Por ahora solo muestra un mensaje indicando que falta implementarlo.

En Visual Studio, abrir `Tr1-BDD-CSharp.sln`, hacer clic derecho sobre el proyecto deseado y seleccionar **Establecer como proyecto de inicio**.

## Organizacion

- `src/`: un proyecto por ejercicio, cada uno con su propio `.csproj` y `Program.cs`.
- Dentro de cada proyecto: `Modelos`, `Servicios`, `Persistencia`, `Reportes` y `Consola`. `Program.cs` es el punto de entrada; `Consola` queda para la futura interfaz de menus.
- `datos-prueba/`: datos de prueba separados por ejercicio.
- `pruebas/`: casos de prueba separados por ejercicio.
- `documentacion/diagramas-de-clases/`: diagramas pendientes.
- `documentacion/informe/`: informe y bitacora de uso de IA pendientes.

Los proyectos no dependen entre si. Cada integrante puede desarrollar el ejercicio asignado dentro de su proyecto y registrar sus propios commits.
