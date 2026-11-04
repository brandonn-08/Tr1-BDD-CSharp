# Ejercicio 2: explicación del diagrama

Abre Ejercicio02.drawio en https://app.diagrams.net usando Archivo > Abrir desde > Dispositivo. Ejercicio02.svg es la versión para ver o insertar en el informe.

- Un Socio puede tener cero o muchas cuentas. SocioId identifica al dueño.
- Cuenta es abstracta: reúne número, socio, saldo, estado y fecha de apertura.
- CuentaAhorro y CuentaCorriente heredan de Cuenta. Cada una define el saldo permitido y el interés mensual.
- CuentaAhorro exige un mínimo de $5 después del primer depósito y calcula saldo × 0,04 / 12.
- CuentaCorriente permite llegar hasta -$200 y devuelve cero de interés.
- Cada Cuenta registra cero o muchos movimientos. CuentaNumero vincula el movimiento con la cuenta.
- Movimiento guarda el monto y el saldo posterior. Una transferencia genera dos movimientos con el mismo TransferenciaId.
- DatosCooperativa agrupa las listas que se guardan en JSON.
- Cooperativa realiza las operaciones y usa ArchivoJson para conservar los datos.
- ReportesCooperativa consulta los datos y genera los cinco reportes del enunciado.

En UML, la flecha con triángulo vacío significa herencia y apunta a Cuenta. Las otras flechas indican relaciones o uso. Los signos + y - indican miembros públicos y privados.

Se separaron los seis modelos en archivos individuales. El menú, las operaciones y las pruebas tienen instrucciones en líneas separadas. Se conservó el formato JSON existente y la protección de las transferencias ante fallos.
