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
- Dentro de cada proyecto: `Modelos`, `Servicios`, `Persistencia` y `Reportes`. `Program.cs` es el punto de entrada y puede contener el futuro menu. No se necesita una carpeta `Consola` adicional para empezar.
- `datos-prueba/`: datos de prueba separados por ejercicio.
- `pruebas/`: casos de prueba separados por ejercicio.
- `documentacion/diagramas-de-clases/`: diagramas pendientes.
- `documentacion/informe/`: informe y bitacora de uso de IA pendientes.

Los proyectos no dependen entre si. Cada integrante puede desarrollar el ejercicio asignado dentro de su proyecto y registrar sus propios commits.

## Para que sirve cada elemento de la raiz

| Elemento | Uso |
| --- | --- |
| `src/` | Contiene los cinco proyectos independientes y su codigo. |
| `Tr1-BDD-CSharp.sln` | Permite abrir y compilar los cinco proyectos juntos en Visual Studio. |
| `README.md` | Instrucciones de compilacion y ejecucion exigidas en la entrega. El diagrama de clases debe agregarse cuando se disenen las clases. |
| `.gitignore` | Evita subir archivos locales de Visual Studio y resultados de compilacion. |
| `datos-prueba/` | Archivos de datos consistentes exigidos por el enunciado. |
| `pruebas/` | Espacio para registrar los casos y resultados reales que luego iran en el informe; no contiene proyectos de pruebas automatizadas por ahora. |
| `documentacion/` | Espacio para los diagramas de clases y el informe con la bitacora de IA. |

Los nombres de las carpetas son una decision de organizacion del grupo; el enunciado exige los entregables, no estos nombres concretos. Se mantienen para que cada integrante sepa donde colocar su trabajo.

Las carpetas vacias se conservan en Git mediante archivos `.gitkeep`, que no contienen codigo. Las carpetas internas de cada proyecto son una guia inicial y pueden adaptarse durante el desarrollo.
