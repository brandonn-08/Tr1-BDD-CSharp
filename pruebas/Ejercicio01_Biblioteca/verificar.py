"""Pruebas del programa de consola. Ejecutar con Python 3 después de compilar."""
import json
import subprocess
import tempfile
from datetime import date, timedelta
from pathlib import Path

raiz = Path(__file__).resolve().parents[2]
dll = raiz / 'src/Ejercicio01_Biblioteca/bin/Debug/net10.0/Ejercicio01_Biblioteca.dll'
hoy = date.today()
resultados = []

def fecha(dias):
    return (hoy + timedelta(days=dias)).isoformat() + 'T00:00:00'

def base():
    return {'Usuarios': [{'Id': 1, 'Nombre': 'Ana', 'MultaPendiente': 0}],
            'Materiales': [{'Id': 1, 'Titulo': 'Libro A', 'Tipo': 'Libro', 'Total': 5},
                           {'Id': 2, 'Titulo': 'Revista B', 'Tipo': 'Revista', 'Total': 1}],
            'Prestamos': []}

def prestamo(id=1, material=1, vence=-2):
    return {'Id': id, 'UsuarioId': 1, 'MaterialId': material, 'Fecha': fecha(-10),
            'Vence': fecha(vence), 'Devuelto': None, 'Multa': 0}

def ejecutar(datos, entradas):
    with tempfile.TemporaryDirectory(dir=raiz / 'pruebas/Ejercicio01_Biblioteca') as carpeta:
        archivo = Path(carpeta) / 'datos.json'
        archivo.write_text(json.dumps(datos), encoding='utf-8')
        proceso = subprocess.run(['dotnet', str(dll), str(archivo)],
                                 input=entradas + '\n0\n', capture_output=True, text=True,
                                 encoding='utf-8', timeout=15)
        assert proceso.returncode == 0, proceso.stderr
        return json.loads(archivo.read_text(encoding='utf-8')), proceso.stdout

def caso(nombre, esperado, prueba):
    try:
        prueba()
        resultados.append((nombre, esperado, 'Cumple', 'Correcto'))
    except Exception as error:
        resultados.append((nombre, esperado, str(error) or 'La comprobación no coincide', 'Falló'))

def comprobar(condicion):
    assert condicion

def plazo(tipo, dias):
    datos, salida = ejecutar(base(), '3\n1\n' + str(tipo))
    comprobar(datos['Prestamos'][0]['Vence'][:10] == (hoy + timedelta(days=dias)).isoformat())

caso('Plazo de libro', 'Vence dentro de 7 días', lambda: plazo(1, 7))
caso('Plazo de revista', 'Vence dentro de 3 días', lambda: plazo(2, 3))

def limite():
    datos = base()
    datos['Prestamos'] = [prestamo(i) for i in range(1, 4)]
    final, salida = ejecutar(datos, '3\n1\n1')
    comprobar(len(final['Prestamos']) == 3 and '3 préstamos activos' in salida)
caso('Cuarto préstamo', 'Se rechaza sin crear otro préstamo', limite)

def sin_stock():
    datos = base()
    datos['Prestamos'] = [prestamo(material=2)]
    final, salida = ejecutar(datos, '3\n1\n2')
    comprobar(final == datos and 'No hay ejemplares' in salida)
caso('Sin ejemplares', 'Se rechaza y no modifica datos', sin_stock)

def deuda():
    datos = base()
    datos['Usuarios'][0]['MultaPendiente'] = 1
    final, salida = ejecutar(datos, '3\n1\n1')
    comprobar(final == datos and 'multas pendientes' in salida)
caso('Usuario con deuda', 'No obtiene nuevos préstamos', deuda)

def devolver(material, retraso, multa):
    datos = base()
    datos['Prestamos'] = [prestamo(material=material, vence=-retraso)]
    final, salida = ejecutar(datos, '4\n1\n4\n1\n11')
    comprobar(final['Prestamos'][0]['Multa'] == multa)
    comprobar(final['Usuarios'][0]['MultaPendiente'] == multa)
    comprobar(final['Prestamos'][0]['Devuelto'][:10] == hoy.isoformat())
    comprobar('Ya fue devuelto' in salida)
caso('Libro con 2 días de retraso y devolución repetida', '$0,50 una sola vez', lambda: devolver(1, 2, .5))
caso('Revista con 2 días de retraso y devolución repetida', '$1,00 una sola vez', lambda: devolver(2, 2, 1))
caso('Devolución en fecha límite', 'Multa cero', lambda: devolver(1, 0, 0))

def pagos():
    datos = base()
    datos['Usuarios'][0]['MultaPendiente'] = 2
    final, salida = ejecutar(datos, '5\n1\n-1\n5\n1\n3\n5\n1\n1\n5\n1\n1\n3\n1\n1')
    comprobar(final['Usuarios'][0]['MultaPendiente'] == 0 and len(final['Prestamos']) == 1)
    comprobar(salida.count('Pago inválido') == 2)
caso('Pagos y desbloqueo', 'Rechaza montos inválidos y permite prestar al saldar', pagos)

def reportes():
    datos = base()
    datos['Prestamos'] = [prestamo(), prestamo(2, 1, 3)]
    final, salida = ejecutar(datos, '7\n8\n1\n9\n11')
    comprobar('#1 Ana - Libro A' in salida and '#2 Ana - Libro A' in salida)
    comprobar('Libro A: 2 préstamos' in salida and 'total 5, prestados 2, disponibles 3' in salida)
    # Consulta aislada para comprobar que el préstamo vigente no se marca vencido.
    final, vencidos = ejecutar(datos, '7')
    comprobar('#1 Ana' in vencidos and '#2 Ana' not in vencidos)
caso('Vencidos, historial, más prestados y auditoría', 'Listados y cantidades consistentes', reportes)

def periodo():
    datos = base()
    p = prestamo()
    p['Devuelto'] = fecha(0)
    p['Multa'] = .5
    datos['Prestamos'] = [p]
    datos['Usuarios'][0]['MultaPendiente'] = .5
    final, salida = ejecutar(datos, f'10\n{hoy}\n{hoy}')
    comprobar('0,50' in salida or '0.50' in salida)
    final, salida = ejecutar(datos, f'10\n{hoy + timedelta(days=1)}\n{hoy}')
    comprobar('Fechas inválidas' in salida)
caso('Multas por período', 'Incluye ambos límites y rechaza período invertido', periodo)

def invalidos():
    datos = base()
    final, salida = ejecutar(datos, '3\n999\n1\n4\n999\n2\nInválido\n1\n0')
    comprobar(final == datos)
caso('IDs y cantidades inválidas', 'No modifica datos', invalidos)

def persistencia():
    datos, salida = ejecutar(base(), '1\nLuis\n3\n2\n1')
    final, salida = ejecutar(datos, '6\n8\n2')
    comprobar('Luis' in salida and '#1 Luis' in salida and final == datos)
caso('Persistencia', 'Conserva usuarios y préstamos al volver a iniciar', persistencia)

informe = '# Resultados de pruebas: ejercicio 1\n\n'
informe += f'Ejecución real: {hoy}. Pruebas aisladas mediante el menú y archivos JSON temporales.\n\n'
informe += '| Caso | Resultado esperado | Resultado real | Estado |\n|---|---|---|---|\n'
for fila in resultados:
    informe += '| ' + ' | '.join(fila) + ' |\n'
destino = raiz / 'documentacion/informe/Pruebas-Ejercicio01.md'
destino.write_text(informe, encoding='utf-8')
print(informe)
raise SystemExit(1 if any(fila[3] == 'Falló' for fila in resultados) else 0)
