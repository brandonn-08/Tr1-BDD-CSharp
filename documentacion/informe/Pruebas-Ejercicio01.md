# Resultados de pruebas: ejercicio 1

Ejecución real: 2026-11-04. Pruebas aisladas mediante el menú y archivos JSON temporales.

| Caso | Resultado esperado | Resultado real | Estado |
|---|---|---|---|
| Plazo de libro | Vence dentro de 7 días | Cumple | Correcto |
| Plazo de revista | Vence dentro de 3 días | Cumple | Correcto |
| Cuarto préstamo | Se rechaza sin crear otro préstamo | Cumple | Correcto |
| Sin ejemplares | Se rechaza y no modifica datos | Cumple | Correcto |
| Usuario con deuda | No obtiene nuevos préstamos | Cumple | Correcto |
| Libro con 2 días de retraso y devolución repetida | $0,50 una sola vez | Cumple | Correcto |
| Revista con 2 días de retraso y devolución repetida | $1,00 una sola vez | Cumple | Correcto |
| Devolución en fecha límite | Multa cero | Cumple | Correcto |
| Pagos y desbloqueo | Rechaza montos inválidos y permite prestar al saldar | Cumple | Correcto |
| Vencidos, historial, más prestados y auditoría | Listados y cantidades consistentes | Cumple | Correcto |
| Multas por período | Incluye ambos límites y rechaza período invertido | Cumple | Correcto |
| IDs y cantidades inválidas | No modifica datos | Cumple | Correcto |
| Persistencia | Conserva usuarios y préstamos al volver a iniciar | Cumple | Correcto |
