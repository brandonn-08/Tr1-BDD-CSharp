# Informe del ejercicio 1: biblioteca UTN

## Funcionamiento

El programa permite registrar usuarios, libros y revistas. Cada material tiene una cantidad de ejemplares y cada préstamo relaciona a un usuario con un material. Las opciones del menú permiten prestar, devolver, pagar multas y consultar los reportes.

Los datos se guardan en JSON para conservarlos al cerrar el programa. Antes de prestar se revisa que haya ejemplares, que el usuario no tenga multas pendientes y que tenga menos de tres préstamos activos. Al devolver se calcula la multa según los días de retraso y el tipo de material. No se permite registrar dos veces la misma devolución.

La opción de multas por período usa la fecha de devolución. En ese momento se conoce el retraso final y se registra la multa generada. El pago disminuye la deuda del usuario, pero no modifica el total histórico de multas generadas.

## Pruebas realizadas

Se ejecutaron 13 casos a través del menú de consola, con archivos temporales separados de los datos de uso. Se comprobaron los plazos, las multas, las restricciones, los pagos, los reportes y la persistencia. Los 13 casos pasaron. La tabla completa está en [Pruebas-Ejercicio01.md](Pruebas-Ejercicio01.md). Las pruebas usan la fecha del día en que se ejecutan y generan préstamos anteriores para comprobar los retrasos.

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
