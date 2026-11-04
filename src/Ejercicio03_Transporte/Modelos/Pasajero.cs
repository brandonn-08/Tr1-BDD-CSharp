namespace Ejercicio03_Transporte.Modelos
{
    public enum TipoPasajero
    {
        Normal = 0,
        TerceraEdad = 1,
        Discapacidad = 2
    }

    public class Pasajero
    {
        public string Cedula { get; set; } = "";
        public string Nombre { get; set; } = "";
        public TipoPasajero Tipo { get; set; }

        public Pasajero()
        {
        }

        public Pasajero(string cedula, string nombre, TipoPasajero tipo)
        {
            Cedula = cedula;
            Nombre = nombre;
            Tipo = tipo;
        }
    }
}