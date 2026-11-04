# Ejercicio 1: Biblioteca UTN

Programa de consola en C# con clases sencillas y datos guardados en JSON.

Para ejecutarlo, abre una consola en esta carpeta y escribe `dotnet run`. El programa crea `biblioteca.json` allí. Si deseas usar otro archivo, ejecuta `dotnet run -- ruta-del-archivo.json`.

Los libros se prestan por 7 días y tienen una multa de $0,25 por día de retraso. Las revistas se prestan por 3 días y tienen una multa de $0,50 por día. El programa controla el máximo de 3 préstamos activos, las multas pendientes, los ejemplares disponibles y las devoluciones repetidas. Las opciones 7 a 11 muestran los cinco reportes pedidos. El total de multas por período usa la fecha de devolución, que es cuando se calcula la multa.

El diagrama de clases está en `documentacion/diagramas-de-clases/Ejercicio01-Biblioteca.png` y se puede abrir directamente desde VS Code o GitHub.
