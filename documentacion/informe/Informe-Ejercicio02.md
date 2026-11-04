# Informe - Ejercicio 2

Autor del aporte: Arévalo Bustamante Dany Andrés. Fecha: 4 de noviembre de 2026.

Docente: Ing. Marco R. Pusdá-Chulde, PhD.

Repositorio: https://github.com/brandonn-08/Tr1-BDD-CSharp


## 1. Alcance y solución

Se desarrolló una aplicación de consola en C# con archivos JSON. Permite registrar socios y varias cuentas por socio; realizar depósitos, retiros y transferencias; aplicar intereses mensuales y cerrar cuentas. Este documento corresponde únicamente al ejercicio 2: los otros cuatro ejercicios y la integración del informe grupal siguen pendientes.

Cuenta es una clase abstracta; CuentaAhorro y CuentaCorriente implementan límites y cálculo de interés distintos. Cooperativa valida las operaciones. ArchivoJson carga y guarda el estado. ReportesCooperativa genera estado de cuenta, intereses del mes, sobregiros, totales por período y auditoría de saldos.


## 2. Reglas y decisiones

Ahorro: mínimo $5 después del primer depósito; interés mensual = saldo de fin de mes × 0,04 / 12, redondeado a centavos. Corriente: sin interés y sobregiro máximo de $200. Los montos deben ser positivos y tener como máximo dos decimales.

Las cuentas se abren en cero, pendientes de financiación; el primer depósito a ahorro debe alcanzar $5. Una cuenta con saldo diferente de cero no se cierra. Con movimientos se conserva inactiva; sin movimientos se elimina. No se inventó una excepción para retirar los últimos $5 de ahorro: esa liquidación debe consultarse al docente.

Las transferencias se realizan entre cuentas del mismo socio y se registran como retiro y depósito vinculados. Se opera sobre una copia y se reemplaza el archivo completo al finalizar; un fallo de validación o guardado no cambia el estado original. Se admite una sola instancia por archivo.

Los intereses se aplican una vez por mes y cuenta al último día del mes. Se registran primero las operaciones del mes; después del cierre no se admiten movimientos retroactivos. Los períodos son inclusivos y los totales incluyen las transferencias, pero excluyen abonos de interés.


## 3. Compilación y ejecución

Requisito: SDK de .NET 10. Desde la raíz: dotnet build Tr1-BDD-CSharp.sln. Ejecutar: dotnet run --project src/Ejercicio02_CooperativaAhorro. El menú muestra la ruta del JSON utilizado y guarda cada operación aceptada.

Pruebas: dotnet run --project pruebas/Ejercicio02_CooperativaAhorro -- pruebas/Ejercicio02_CooperativaAhorro/RESULTADOS.md. Este es un ejecutable de pruebas sin paquetes externos; no utiliza dotnet test. Código de salida 0 significa que todos los casos pasaron.


## 4. Diagrama de clases resumido

DatosCooperativa agrupa las listas de socios, cuentas y movimientos. El diagrama completo y editable está en el README del ejercicio y en documentacion/diagramas-de-clases/Ejercicio02.mmd.


## 5. Datos consistentes y demostración

El archivo cooperativa-ejemplo.json contiene a los socios ficticios Ana Torres y Luis Mina. Los saldos finales son: ahorro 101 = $80,27; corriente 102 = -$30,00; ahorro 201 = $301,00. En octubre de 2026 los depósitos suman $420,00, los retiros $70,00 y los intereses $1,27. Las tres cuentas pasan la auditoría.

Se ejecutaron los cinco reportes mediante el menú con el archivo de ejemplo; la salida se conserva en pruebas/Ejercicio02_CooperativaAhorro/DEMOSTRACION.txt. Las pruebas automatizadas usan archivos temporales independientes y verifican resultados, rechazos y conservación del estado.


## 6. Bitácora de IA y pendientes de entrega

Herramienta: Codex (OpenAI). Se recibió ayuda amplia para interpretar reglas, generar la implementación, crear y ejecutar pruebas, preparar datos ficticios, diagrama e informe. No se declara que la ayuda se limitó a consultas puntuales. La bitácora detallada está en Bitacora-IA-Ejercicio02.md.

El estudiante debe revisar, ejecutar y poder explicar el código antes de presentarlo. No se dispone del compromiso 7 del sílabo; se debe contrastar la bitácora con ese texto. El grupo debe integrar los otros ejercicios y sus pruebas, registrar los aportes reales de todos los integrantes y producir Apellidos-Tr1.pdf para el portafolio. Este PDF es el aporte individual del ejercicio 2.


## 7. Tabla de casos de prueba con resultados reales

Ejecucion: 2026-11-04 11:26 -05:00

| Caso | Resultado esperado | Resultado real | Estado |
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

La compilación de la solución terminó con 0 errores y 0 advertencias. Estos resultados corresponden a ejecución automatizada asistida; no acreditan una revisión manual del estudiante.
