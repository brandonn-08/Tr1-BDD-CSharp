# Ejercicio 3 · Cooperativa de transporte de Ibarra

Aplicación de consola en C# (.NET 10) para vender y anular boletos de las rutas Ibarra–Quito, Ibarra–Tulcán e Ibarra–Otavalo. La información se guarda en archivos de texto.

## Cómo compilar y ejecutar

Desde la raíz del repositorio:

```
dotnet build Tr1-BDD-CSharp.sln
dotnet run --project src/Ejercicio03_Transporte
```

En Visual Studio: poner `Ejercicio03_Transporte` como proyecto de inicio y presionar F5.

## Datos de ejemplo

Están en la carpeta `datos/` y se copian solos junto al ejecutable:

- `viajes.txt`: cuatro viajes (V01 y V02 son Ibarra → Quito).
- `boletos.txt`: tres boletos, uno de ellos anulado (BOL-0002, con su asiento libre).

Los datos son consistentes: los asientos ocupados de cada viaje corresponden a los boletos activos.

## Reglas principales

- Viaje directo: 100 % de la tarifa. Con paradas: 85 %.
- Tercera edad y discapacidad: 50 % de descuento.
- No se vende un asiento ocupado, fuera de rango ni de un viaje que ya salió.
- Un boleto se anula hasta 2 horas antes de la salida, solo una vez, y libera el asiento.
- No se elimina un viaje que tenga boletos vendidos, aunque estén anulados.

## Reportes

Mapa de asientos, recaudación por ruta en un período, porcentaje de ocupación, boletos anulados en un período y auditoría de asientos libres.

## Diagrama de clases

Está en `documentacion/diagramas-de-clases/Ejercicio03.md`.
