# Ejercicio 5 - Artesanias en consignacion

```powershell
dotnet run --project src/Ejercicio05_Artesanias
```

El menu permite gestionar comunidades, emprendedores y artesanias, registrar recepciones, ventas y devoluciones y consultar los cinco reportes. Los textiles tienen una comision del 20 % y las tallas en madera del 25 %. La venta consume primero los lotes mas antiguos (FIFO), sin superar el stock. La comision de cada detalle se redondea a centavos; los importes y porcentajes de las ventas quedan guardados para conservar el historial.

## Datos

Sin argumentos usa `bin/Debug/net10.0/datos/tienda.json` dentro del proyecto. No escribe sobre el archivo versionado `Datos/tienda.json`. Si ya tenias datos en esa ubicacion antigua, puedes abrirlos pasando la ruta como argumento, o copiarlos a la nueva carpeta local. Para una ubicacion propia:

```powershell
dotnet run --project src/Ejercicio05_Artesanias -- C:\Temp\tienda.json
```

Para usar los datos de prueba sin modificar el original:

```powershell
Copy-Item datos-prueba/Ejercicio05_Artesanias/tienda-ejemplo.json "$env:TEMP\tienda-demo.json"
dotnet run --project src/Ejercicio05_Artesanias -- "$env:TEMP\tienda-demo.json"
```

El ejemplo contiene dos comunidades, dos emprendedores, dos artesanias, dos recepciones, una venta y una devolucion. Quedan dos ponchos y tres tallas. El textil se liquida por $20 vendidos, $4 de comision y $16 para el emprendedor; la madera por $20, $5 y $15. Hay un lote de textil con mas de noventa dias para consultar devoluciones pendientes. Estos calculos consideran las fechas de las operaciones del ejemplo.

## Integridad

Las ventas y devoluciones se validan completas antes de registrarse. No se devuelve a otro propietario, no se cambia el propietario de una artesania con recepciones ni se desactivan artesanias con movimientos o emprendedores con artesanias. Un error al guardar revierte los cambios en memoria de todos los servicios de escritura. El archivo se reemplaza mediante un temporal.

La auditoria comprueba identificadores, referencias a comunidades, propietarios, tipos y lotes, cantidades positivas, stock no negativo y orden de fechas. Los periodos incluyen todo el dia inicial y final. La marca de devolucion se consulta por lote cuando la fecha de recepcion mas noventa dias es anterior a la fecha consultada; no se cambia el stock por consultar ese reporte.

```powershell
dotnet run --project pruebas/Ejercicios03y05 -- pruebas/Ejercicios03y05/RESULTADOS.md
```
