# Trabajo 1 - Bases de Datos

Una solucion con cinco proyectos de consola C# independientes. El ejercicio 2 (Cooperativa de ahorro) esta implementado con persistencia JSON, cinco reportes y pruebas reproducibles. Los ejercicios 1, 3, 4 y 5 conservan su estructura inicial y estan pendientes.

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

Cada comando ejecuta un proyecto distinto. El ejercicio 2 abre el menu de la cooperativa; los otros cuatro muestran un mensaje de implementacion pendiente.

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
| `pruebas/` | Espacio para registrar los casos y resultados reales que luego iran en el informe; incluye el programa de pruebas automatizadas de consola del ejercicio 2. |
| `documentacion/` | Espacio para los diagramas de clases y el informe con la bitacora de IA. |

Los nombres de las carpetas son una decision de organizacion del grupo; el enunciado exige los entregables, no estos nombres concretos. Se mantienen para que cada integrante sepa donde colocar su trabajo.

Las carpetas vacias se conservan en Git mediante archivos `.gitkeep`, que no contienen codigo. Las carpetas internas de cada proyecto son una guia inicial y pueden adaptarse durante el desarrollo.

## Ejercicio 2 implementado

- [Instrucciones, decisiones y diagrama de clases](src/Ejercicio02_CooperativaAhorro/README.md#diagrama-de-clases).
- [Datos de prueba](datos-prueba/Ejercicio02_CooperativaAhorro/cooperativa-ejemplo.json).
- [Resultados reales de las pruebas](pruebas/Ejercicio02_CooperativaAhorro/RESULTADOS.md).
- [Informe del aporte](documentacion/informe/Arevalo-Bustamante-Tr1-Ejercicio02.pdf).
- [Bitacora de IA](documentacion/informe/Bitacora-IA-Ejercicio02.md).

```powershell
dotnet run --project pruebas/Ejercicio02_CooperativaAhorro -- pruebas/Ejercicio02_CooperativaAhorro/RESULTADOS.md
```

La solucion contiene cinco aplicaciones de ejercicios y un proyecto adicional para las pruebas del ejercicio 2. El informe grupal final y los aportes de los demas integrantes siguen pendientes.
