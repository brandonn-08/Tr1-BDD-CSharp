# Pruebas del ejercicio 4

Ejecución real: 2026-11-04. Comando: `dotnet run --project pruebas/Ejercicio04_Cafeteria/PruebasCafeteria.csproj`.

| Caso | Resultado observado |
| --- | --- |
| Precio del plato e IVA de bebida | Correcto: costo por 2 e IVA del 15 % |
| Venta válida | Correcto: guarda total, descuenta stock y registra movimiento |
| Bebida repetida en la misma venta | Correcto: rechaza la venta y conserva stock |
| Ingredientes insuficientes | Correcto: rechaza toda la venta, sin descontar bebida |
| Tipo de producto inválido | Correcto: rechaza la venta |
| Último día del reporte | Correcto: incluye una venta de las 17:30 y rechaza período invertido |
| Preparación y lista de compras | Correcto: calcula unidades y detecta stock bajo el mínimo |
| Ingrediente usado por receta | Correcto: impide eliminarlo |

**Resultado: 8/8 pruebas correctas.**

Estas pruebas comprueban las reglas principales y los errores corregidos; la ejecución manual del menú y el resto de combinaciones de datos deben revisarse antes de la entrega grupal.
