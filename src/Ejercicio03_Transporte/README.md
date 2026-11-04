# Ejercicio 3 - Transporte

Desde la raiz del repositorio:

```powershell
dotnet run --project src/Ejercicio03_Transporte
```

Se registran viajes de Ibarra a Quito, Tulcan y Otavalo, ventas, anulaciones y los cinco reportes del enunciado. Para los reportes por periodo se ingresan las fechas en formato `aaaa-mm-dd`; ambos dias se incluyen completos. La recaudacion muestra los boletos actualmente vigentes y excluye los anulados.

## Archivos

Sin argumentos usa `bin/Debug/net10.0/datos` dentro del proyecto. Se pueden indicar otros datos:

```powershell
dotnet run --project src/Ejercicio03_Transporte -- C:\Temp\transporte
```

La primera ejecucion sin datos crea tres viajes de ejemplo con salidas futuras. El guardado usa un unico `transporte.json` y un archivo temporal para mantener viajes y boletos sincronizados. Si no existe JSON, importa `viajes.txt` y `boletos.txt` de esa misma carpeta. No modifica los TXT originales. Si los archivos estan corruptos o sus asientos no coinciden, informa el error y no los reemplaza.

Para probar los datos del repositorio, primero copie los dos TXT a una carpeta temporal:

```powershell
New-Item -ItemType Directory -Force "$env:TEMP\transporte-demo"
Copy-Item datos-prueba/Ejercicio03_Transporte/datos/*.txt "$env:TEMP\transporte-demo"
dotnet run --project src/Ejercicio03_Transporte -- "$env:TEMP\transporte-demo"
```

Los viajes de este ejemplo salen en enero de 2027. Despues de esas fechas sirven para consultar reportes y comprobar el rechazo de ventas de viajes que ya salieron.

## Validaciones y pruebas

No permite rutas, tarifas, capacidades ni pasajeros invalidos; no duplica asientos ni identificadores. Un boleto se puede anular exactamente dos horas antes de salir y no se puede anular dos veces. Un viaje con historial de boletos no se elimina. La auditoria compara los asientos guardados con los boletos activos y detecta diferencias, duplicados y asientos fuera de rango. Una operacion cuyo guardado falla revierte el estado en memoria en el menu.

```powershell
dotnet run --project pruebas/Ejercicios03y05 -- pruebas/Ejercicios03y05/RESULTADOS.md
```

Las pruebas usan archivos temporales y un reloj fijo para los limites de tiempo; tambien ejecutan los menus como procesos separados.
