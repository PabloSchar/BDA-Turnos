using System.Numerics;

namespace BDA_Turnos.Models
{
    public class Paciente
    {
        public Usuario Usuario { get; set; }
        public int UsuarioId { get; set; }
        public int DNI { get; set; }
        public DateOnly FechaNacimiento { get; set; }
        public string Direccion { get; set; }
    }
}
