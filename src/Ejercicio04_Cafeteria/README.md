# Ejercicio 4: Cafetería UTN

Aplicación de consola en C# con ingredientes, platos y recetas, bebidas, abastecimientos, ventas e inventario.

## Ejecutar

Desde la raíz del repositorio, con el SDK de .NET 10 instalado:

```powershell
dotnet run --project src/Ejercicio04_Cafeteria
```

El menú permite registrar ingredientes en kg o litros, asignar recetas a los platos, registrar bebidas, reabastecer, vender y consultar reportes. El precio del plato es el doble del costo de sus ingredientes; a las bebidas se suma el 15 % de IVA.

Los datos de ejemplo están en `Persistencia/Datos/` y se copian al directorio de salida al compilar. El programa guarda allí los cambios. Para volver al ejemplo inicial, se puede borrar la carpeta `bin` del ejercicio y compilar otra vez.

## Reglas principales

- Una venta se rechaza completa si falta algún ingrediente o bebida. Se suman las cantidades de los productos repetidos antes de descontar el stock.
- Una receta solo puede usar ingredientes existentes y cantidades positivas.
- No se elimina un ingrediente que se usa en alguna receta.
- Los reportes muestran costo, precio y margen de platos; ingredientes por comprar; platos que se pueden preparar; ventas de platos por período; y movimientos de inventario.

## Pruebas

```powershell
dotnet run --project pruebas/Ejercicio04_Cafeteria/PruebasCafeteria.csproj
```

Las pruebas usan datos aislados en la carpeta de salida del proyecto de pruebas. No cambian los datos de ejemplo del proyecto principal. Sus resultados están en `pruebas/Ejercicio04_Cafeteria/RESULTADOS.md`.
