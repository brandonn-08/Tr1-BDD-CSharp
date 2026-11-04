# Resultados reales - Ejercicio 2

Ejecucion: 2026-11-04 11:26 -05:00

Comando: `dotnet run --project pruebas/Ejercicio02_CooperativaAhorro -- pruebas/Ejercicio02_CooperativaAhorro/RESULTADOS.md`

| Caso | Esperado | Resultado real | Estado |
| --- | --- | --- | --- |
| Socio con varias cuentas | 2 cuentas para Ana | 2 cuentas | OK |
| Socio repetido | Rechazo | Rechazado | OK |
| Datos de socio incompletos | Rechazo | Rechazado | OK |
| Cuenta sin socio | Rechazo | Rechazado | OK |
| Numero de cuenta repetido | Rechazo | Rechazado | OK |
| Deposito valido | Saldo 125.50 | 125.50 | OK |
| Deposito invalido 0 | Rechazo sin cambios | Rechazado; estado intacto | OK |
| Retiro invalido 0 | Rechazo sin cambios | Rechazado; estado intacto | OK |
| Deposito invalido -1 | Rechazo sin cambios | Rechazado; estado intacto | OK |
| Retiro invalido -1 | Rechazo sin cambios | Rechazado; estado intacto | OK |
| Deposito invalido 1.001 | Rechazo sin cambios | Rechazado; estado intacto | OK |
| Retiro invalido 1.001 | Rechazo sin cambios | Rechazado; estado intacto | OK |
| Primer deposito de ahorro menor a 5 | Rechazo | Rechazado | OK |
| Primer deposito de ahorro igual a 5 | Saldo 5 | 5.00 | OK |
| Retiro deja minimo de ahorro | Saldo 5 | 5.00 | OK |
| Retiro rompe minimo de ahorro | Rechazo; saldo 100 | Rechazado; 100.00 | OK |
| Sobregiro exacto | Saldo -200 | -200.00 | OK |
| Sobregiro excedido | Rechazo | Rechazado; 0.00 | OK |
| Transferencia valida y vinculada | 70 y 30; retiro/deposito con mismo id | 70.00 / 30.00; vinculada | OK |
| Transferencia misma cuenta | Rechazo sin cambios | Rechazado; intacto | OK |
| Transferencia entre socios distintos | Rechazo | Rechazado | OK |
| Transferencia sin fondos | Ninguna cuenta cambia | Estado y movimientos intactos | OK |
| Transferencia a cuenta inexistente | Ninguna cuenta cambia | Estado intacto | OK |
| Transferencia con destino bajo minimo | Ninguna cuenta cambia | Estado intacto | OK |
| Fallo de segunda operacion por fecha | Retiro de origen tampoco se conserva | Estado intacto | OK |
| Interes anual 4 por ciento mensual | 0.33 en ahorro; 0 en corriente | Interes 0.33; corriente 0.00 | OK |
| Interes repetido | No se duplica | Sin cambios al repetir | OK |
| Interes compuesto mes siguiente | Saldo 100.66 | 100.66 | OK |
| Cuenta futura sin interes | Sin abono anterior a apertura | 0 movimientos | OK |
| Cerrar saldo distinto de cero | Rechazo | Rechazado | OK |
| Cerrar sobregiro | Rechazo | Rechazado | OK |
| Cerrar sin movimientos | Eliminada | Eliminada | OK |
| Cerrar con movimientos y saldo cero | Conservada e inactiva | Inactiva; 2 movimientos conservados | OK |
| Operar cuenta inactiva | Deposito, retiro y transferencia rechazados | 3 rechazos; estado intacto | OK |
| Fecha anterior a apertura | Rechazo | Rechazado | OK |
| Movimiento despues de cierre mensual | Rechaza fecha del mes cerrado | Rechazado | OK |
| Estado de cuenta | Saldos 100, 110, 105 | 100,110,105 | OK |
| Reporte intereses historico | Calculado y acreditado 0.33 | 0.33 / 0.33 | OK |
| Reporte sobregiros | Solo cuenta 102 | 102: -25.00 | OK |
| Totales e inclusion de transferencias | Depositos 120; retiros 20; interes excluido | 120.00 / 20.00 | OK |
| Limites de periodo inclusivos | Solo 10 del dia consultado | 10.00 | OK |
| Periodo invertido | Rechazo | Rechazado | OK |
| Auditoria correcta | Todas las cuentas correctas | 3 cuentas correctas | OK |
| Auditoria detecta saldo alterado | Cuenta 101 incorrecta | Diferencia detectada | OK |
| Auditoria detecta saldo posterior alterado | Historial incorrecto | Historial alterado detectado | OK |
| Persistencia y tipos de cuenta | Mismo estado al reabrir JSON | JSON y tipo conservados | OK |
| Archivo JSON danado | Error; contenido conservado | Error de lectura; sin sobrescritura | OK |
| Error al guardar transferencia | Archivo previo y memoria intactos | Fallo controlado; memoria y respaldo intactos | OK |

Total: 48; correctos: 48; fallidos: 0.
