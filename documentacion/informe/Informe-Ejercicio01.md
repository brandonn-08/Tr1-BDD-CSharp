# Informe del ejercicio 1: biblioteca UTN

## Objetivo y funcionamiento

El ejercicio consiste en administrar los préstamos de una biblioteca mediante una aplicación de consola en C#. El programa permite registrar usuarios, libros y revistas. Cada material tiene una cantidad de ejemplares y cada préstamo relaciona a un usuario con un material. Desde el menú se pueden realizar préstamos, registrar devoluciones, pagar multas y consultar el estado de la biblioteca.

Se usaron cuatro clases sencillas: `Usuario` guarda los datos del lector y su deuda; `Material` representa un libro o una revista y su número de ejemplares; `Prestamo` registra las fechas, la devolución y la multa; y `Datos` agrupa las listas que se guardan en el archivo JSON. El diagrama de estas clases está en [Ejercicio01-Biblioteca.png](../diagramas-de-clases/Ejercicio01-Biblioteca.png).

Los datos se guardan en `biblioteca.json` para conservarlos al cerrar el programa. Se puede indicar otra ruta al ejecutarlo, lo que también permite probarlo sin modificar los datos de uso. Hay un [archivo de ejemplo](../../datos-prueba/Ejercicio01_Biblioteca/ejemplo.json) con dos usuarios, un libro, una revista y dos préstamos.

## Reglas aplicadas

| Operación | Regla implementada |
| --- | --- |
| Prestar un libro | El plazo es de 7 días desde la fecha del préstamo. |
| Prestar una revista | El plazo es de 3 días desde la fecha del préstamo. |
| Validar al usuario | No se presta si tiene una multa pendiente o ya tiene tres préstamos activos. |
| Validar el material | Debe existir al menos un ejemplar disponible; se cuentan los préstamos que todavía no fueron devueltos. |
| Devolver | Una devolución solo se registra una vez. Los días de retraso se cuentan después de la fecha de vencimiento. |
| Calcular la multa | Un libro genera $0,25 por día de retraso y una revista $0,50. Si se devuelve a tiempo, la multa es cero. |
| Pagar | El pago debe ser positivo y no superar la deuda. Al saldarla, el usuario puede volver a prestar. |

La multa generada queda guardada en el préstamo y también aumenta la deuda pendiente del usuario. Un pago disminuye esa deuda, pero no borra la multa histórica del préstamo.

## Reportes de consola

| Opción | Qué muestra |
| --- | --- |
| 7. Vencidos | Préstamos activos cuya fecha de vencimiento ya pasó. |
| 8. Historial | Préstamos activos y devueltos de un usuario. |
| 9. Más prestados | Materiales ordenados por la cantidad de préstamos registrados. |
| 10. Multas por período | Total de multas generadas por devoluciones dentro de las fechas ingresadas. |
| 11. Auditoría | Total, prestados y disponibles de cada material. |

El reporte de multas usa la **fecha de devolución**, porque es cuando se conoce y registra el retraso final. Incluye los dos días indicados por el usuario y rechaza fechas inválidas o un período invertido.

## Pruebas realizadas

Primero se compiló `src/Ejercicio01_Biblioteca/Ejercicio01_Biblioteca.csproj`: el resultado fue **0 errores y 0 advertencias**. Después se ejecutó `python pruebas/Ejercicio01_Biblioteca/verificar.py` desde la raíz del repositorio. La prueba envía opciones al menú y usa archivos JSON temporales separados de los datos normales. Las fechas se calculan a partir del día de ejecución, de modo que los casos de retraso se puedan repetir otro día.

| Caso | Resultado esperado | Resultado observado |
| --- | --- | --- |
| Plazo de libro | Vencimiento a los 7 días. | Correcto. |
| Plazo de revista | Vencimiento a los 3 días. | Correcto. |
| Cuarto préstamo | Rechazarlo sin crear otro registro. | Correcto. |
| Material sin ejemplares | Rechazar el préstamo sin cambiar los datos. | Correcto. |
| Usuario con deuda | Impedir un nuevo préstamo. | Correcto. |
| Libro devuelto dos días tarde | Generar $0,50 una sola vez, aunque se intente devolver de nuevo. | Correcto. |
| Revista devuelta dos días tarde | Generar $1,00 una sola vez, aunque se intente devolver de nuevo. | Correcto. |
| Devolución el día límite | No generar multa. | Correcto. |
| Pagos y desbloqueo | Rechazar pagos inválidos y permitir prestar al saldar. | Correcto. |
| Vencidos, historial, más prestados y auditoría | Mostrar listados y cantidades consistentes. | Correcto. |
| Multas por período | Incluir ambos límites y rechazar un período invertido. | Correcto. |
| Identificadores y cantidades inválidas | No modificar los datos. | Correcto. |
| Persistencia | Conservar usuarios y préstamos al iniciar otra vez. | Correcto. |

**Resultado: 13 de 13 casos correctos.** El detalle de la ejecución está en [Pruebas-Ejercicio01.md](Pruebas-Ejercicio01.md). Estas pruebas cubren las reglas principales del ejercicio; no sustituyen todas las combinaciones posibles de entradas.

## Cómo repetir la comprobación

Desde la carpeta principal del repositorio:

```powershell
dotnet build src/Ejercicio01_Biblioteca/Ejercicio01_Biblioteca.csproj
python pruebas/Ejercicio01_Biblioteca/verificar.py
```

Para usar el programa manualmente: `dotnet run --project src/Ejercicio01_Biblioteca`. Los pasos básicos son registrar un usuario y un material, prestar un ejemplar y después consultar el listado o el historial. Las instrucciones de uso también están en el [README del ejercicio](../../src/Ejercicio01_Biblioteca/README.md).

## Bitácora de uso de IA

Esta sección describe el apoyo de IA en tres métodos del ejercicio.

| Método | Apoyo de IA | Resultado |
|---|---|---|
| Prestar | Usé IA para elaborar las validaciones del préstamo: revisar las multas pendientes, contar los préstamos activos y comprobar si hay ejemplares disponibles. También recibí apoyo para calcular la fecha de vencimiento según el tipo de material. | El método rechaza los préstamos que no cumplen las reglas y registra los aceptados con su fecha límite. |
| Devolver | Usé IA para elaborar el cálculo de los días de retraso y aplicar la tarifa correspondiente a libros o revistas. También recibí apoyo para evitar que una devolución se registre dos veces. | El método registra la devolución, calcula la multa y actualiza la deuda del usuario. |
| MultasPeriodo | Usé IA para elaborar la validación de las fechas y sumar las multas de las devoluciones realizadas dentro del período indicado, incluyendo las fechas inicial y final. | El reporte muestra las multas generadas en el período y rechaza las fechas inválidas o un período invertido. |

## Repositorio

[Tr1-BDD-CSharp](https://github.com/brandonn-08/Tr1-BDD-CSharp)

El enlace corresponde al repositorio grupal donde se comparte este aporte del ejercicio 1. El informe final debe reunir los resultados de los cinco ejercicios y los apellidos del grupo antes de exportarlo como `Apellidos-Tr1.pdf`.
