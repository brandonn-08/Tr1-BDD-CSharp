# Aporte del ejercicio 4: Cafetería UTN

El programa maneja ingredientes, recetas, platos, bebidas, abastecimientos y ventas con archivos JSON. Incluye cinco consultas: costo y margen por plato, ingredientes para comprar, cantidad de platos preparables, ventas por período y auditoría de movimientos de ingredientes.

Se corrigieron tres problemas encontrados al probarlo: el menú pedía varias veces la cantidad de elementos, una bebida repetida en la misma venta podía dejar stock negativo y el reporte de ventas omitía registros del último día del período. También se agregó validación del tipo de producto y del período invertido.

Las pruebas automáticas se encuentran en `pruebas/Ejercicio04_Cafeteria/`. El resultado observado fue 8/8 casos correctos el 2026-11-04. Los archivos de ejemplo están en `src/Ejercicio04_Cafeteria/Persistencia/Datos/` y las instrucciones para ejecutar el programa están en su README.

## Bitácora de apoyo de IA

Se utilizó IA para revisar el flujo de venta y detectar que las cantidades de una misma bebida debían sumarse antes de validar el stock. También ayudó a preparar las pruebas de venta completa y a corregir el filtro de fechas del reporte. Esta sección documenta el apoyo recibido en esas correcciones; el integrante debe revisarla y completarla con su experiencia real antes de incorporarla al informe grupal.
