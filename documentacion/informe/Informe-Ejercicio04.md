# Aporte del ejercicio 4: Cafetería UTN

El programa maneja ingredientes, recetas, platos, bebidas, abastecimientos y ventas con archivos JSON. Incluye cinco consultas: costo y margen por plato, ingredientes para comprar, cantidad de platos preparables, ventas por período y auditoría de movimientos de ingredientes.

Se corrigieron tres problemas encontrados al probarlo: el menú pedía varias veces la cantidad de elementos, una bebida repetida en la misma venta podía dejar stock negativo y el reporte de ventas omitía registros del último día del período. También se agregó validación del tipo de producto y del período invertido.

Las pruebas automáticas se encuentran en `pruebas/Ejercicio04_Cafeteria/`. El resultado observado fue 8/8 casos correctos el 2026-11-04. Los archivos de ejemplo están en `src/Ejercicio04_Cafeteria/Persistencia/Datos/` y las instrucciones para ejecutar el programa están en su README.

## Bitácora de apoyo de IA

1. **`VentaService.Registrar`:** Se usó IA para entender cómo sumar las cantidades cuando una misma bebida aparece varias veces en una venta. Con esa lógica se valida el stock total antes de descontarlo y se evita guardar una venta incompleta.
2. **`ReporteVentas.Obtener`:** Se usó IA para entender el filtro de fechas. El reporte debía incluir las ventas realizadas durante todo el último día seleccionado y rechazar un período con las fechas invertidas.
3. **`Program.RegistrarVenta`:** Se usó IA para revisar el ciclo que pide los productos de una venta. La cantidad de productos se lee una sola vez antes del ciclo, para que el programa no vuelva a pedirla en cada vuelta.
