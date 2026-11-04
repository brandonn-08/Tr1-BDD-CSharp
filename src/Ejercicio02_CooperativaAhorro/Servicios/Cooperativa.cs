using Ejercicio02_CooperativaAhorro.Modelos;
using Ejercicio02_CooperativaAhorro.Persistencia;

namespace Ejercicio02_CooperativaAhorro.Servicios
{
    public class Cooperativa
    {
        private DatosCooperativa datos;
        private readonly ArchivoJson archivo;
        public Cooperativa(string ruta)
        {
            archivo = new ArchivoJson(ruta);
            datos = archivo.Cargar();
        }

        public DatosCooperativa Consultar()
        {
            return ArchivoJson.Copiar(datos);
        }

        private static Cuenta Buscar(DatosCooperativa d, int numero, bool activa = true)
        {
            var c = d.Cuentas.SingleOrDefault(c => c.Numero == numero) ?? throw new InvalidOperationException("La cuenta no existe.");
            if (activa && !c.Activa)
            {
                throw new InvalidOperationException("La cuenta esta inactiva.");
            }

            return c;
        }

        private static void MontoValido(decimal monto)
        {
            if (monto <= 0 || decimal.Round(monto, 2) != monto)
            {
                throw new InvalidOperationException("El monto debe ser positivo y tener como maximo 2 decimales.");
            }
        }

        private static void Registrar(DatosCooperativa d, Cuenta c, DateOnly fecha, string tipo, decimal monto, string detalle = "", string? transferencia = null)
        {
            if (fecha < c.Apertura || d.Movimientos.Any(m => m.CuentaNumero == c.Numero && m.Fecha > fecha))
            {
                throw new InvalidOperationException("La fecha no puede ser anterior a la apertura o al ultimo movimiento.");
            }

            if (tipo != "Interes" && d.Movimientos.Any(m => m.CuentaNumero == c.Numero && m.Tipo == "Interes" && m.Fecha >= fecha))
            {
                throw new InvalidOperationException("El mes ya tiene intereses aplicados; use una fecha posterior al cierre mensual.");
            }

            c.Saldo += tipo == "Retiro" ? -monto : monto;
            Movimiento movimiento = new Movimiento();
            movimiento.Id = d.Movimientos.Select(m => m.Id).DefaultIfEmpty(0).Max() + 1;
            movimiento.CuentaNumero = c.Numero;
            movimiento.Fecha = fecha;
            movimiento.Tipo = tipo;
            movimiento.Monto = monto;
            movimiento.SaldoPosterior = c.Saldo;
            movimiento.Detalle = detalle;
            movimiento.TransferenciaId = transferencia;
            d.Movimientos.Add(movimiento);
        }

        public void AgregarSocio(int id, string nombre)
        {
            DatosCooperativa d = ArchivoJson.Copiar(datos);
            if (id <= 0 || string.IsNullOrWhiteSpace(nombre))
            {
                throw new InvalidOperationException("Id y nombre obligatorios.");
            }

            if (d.Socios.Any(s => s.Id == id))
            {
                throw new InvalidOperationException("El socio ya existe.");
            }

            Socio nuevoSocio = new Socio();
            nuevoSocio.Id = id;
            nuevoSocio.Nombre = nombre.Trim();
            d.Socios.Add(nuevoSocio);
            archivo.Guardar(d);
            datos = d;
        }

        public void AbrirCuenta(int numero, int socio, bool ahorro, DateOnly fecha)
        {
            DatosCooperativa d = ArchivoJson.Copiar(datos);
            if (!d.Socios.Any(s => s.Id == socio))
            {
                throw new InvalidOperationException("El socio no existe.");
            }

            if (numero <= 0 || d.Cuentas.Any(c => c.Numero == numero))
            {
                throw new InvalidOperationException("Numero de cuenta invalido o repetido.");
            }

            Cuenta c;
            if (ahorro)
            {
                c = new CuentaAhorro();
            }
            else
            {
                c = new CuentaCorriente();
            }

            c.Numero = numero;
            c.SocioId = socio;
            c.Apertura = fecha;
            d.Cuentas.Add(c);
            archivo.Guardar(d);
            datos = d;
        }

        public void Depositar(int numero, decimal monto, DateOnly fecha)
        {
            DatosCooperativa d = ArchivoJson.Copiar(datos);
            MontoValido(monto);
            var c = Buscar(d, numero);
            if (!c.PermiteSaldo(c.Saldo + monto))
            {
                throw new InvalidOperationException("El deposito no alcanza el saldo minimo de $5.");
            }

            Registrar(d, c, fecha, "Deposito", monto);
            archivo.Guardar(d);
            datos = d;
        }

        public void Retirar(int numero, decimal monto, DateOnly fecha)
        {
            DatosCooperativa d = ArchivoJson.Copiar(datos);
            MontoValido(monto);
            var c = Buscar(d, numero);
            if (!c.PermiteSaldo(c.Saldo - monto))
            {
                throw new InvalidOperationException("Saldo insuficiente o limite de sobregiro excedido.");
            }

            Registrar(d, c, fecha, "Retiro", monto);
            archivo.Guardar(d);
            datos = d;
        }

        public void Transferir(int socio, int origen, int destino, decimal monto, DateOnly fecha)
        {
            DatosCooperativa d = ArchivoJson.Copiar(datos);
            MontoValido(monto);
            if (origen == destino)
            {
                throw new InvalidOperationException("Las cuentas deben ser distintas.");
            }

            var a = Buscar(d, origen);
            var b = Buscar(d, destino);
            if (a.SocioId != socio || b.SocioId != socio)
            {
                throw new InvalidOperationException("Ambas cuentas deben pertenecer al socio.");
            }

            if (!a.PermiteSaldo(a.Saldo - monto) || !b.PermiteSaldo(b.Saldo + monto))
            {
                throw new InvalidOperationException("La transferencia incumple los limites de saldo.");
            }

            string id = Guid.NewGuid().ToString("N");
            Registrar(d, a, fecha, "Retiro", monto, $"Transferencia a {destino}", id);
            Registrar(d, b, fecha, "Deposito", monto, $"Transferencia desde {origen}", id);
            archivo.Guardar(d);
            datos = d;
        }

        public void AplicarIntereses(int anio, int mes)
        {
            DatosCooperativa d = ArchivoJson.Copiar(datos);
            var fecha = new DateOnly(anio, mes, DateTime.DaysInMonth(anio, mes));
            foreach (var c in d.Cuentas.OfType<CuentaAhorro>().Where(c => c.Activa && c.Apertura <= fecha))
            {
                if (d.Movimientos.Any(m => m.CuentaNumero == c.Numero && m.Tipo == "Interes" && m.Fecha.Year == anio && m.Fecha.Month == mes))
                {
                    continue;
                }

                // Registrar incluso un interes cero evita aplicar el mismo mes dos veces.
                Registrar(d, c, fecha, "Interes", c.InteresMensual(), $"Interes {anio}-{mes:00}");
            }

            archivo.Guardar(d);
            datos = d;
        }

        public void CerrarCuenta(int numero)
        {
            DatosCooperativa d = ArchivoJson.Copiar(datos);
            var c = Buscar(d, numero);
            if (c.Saldo != 0)
            {
                throw new InvalidOperationException("Solo se puede cerrar una cuenta con saldo cero.");
            }

            if (d.Movimientos.Any(m => m.CuentaNumero == numero))
            {
                c.Activa = false;
            }
            else
                d.Cuentas.Remove(c);
            archivo.Guardar(d);
            datos = d;
        }
    }
}
