using System.ComponentModel.DataAnnotations.Schema;

namespace BDA_Turnos.Models
{
    public class Turno
    {
        public int TurnoId { get; set; }
        public Paciente Paciente { get; set; }
        public int PacienteId { get; set; }
        public Medico Medico { get; set; }
        public int MedicoId { get; set; }
        public DateOnly Fecha { get; set; }
        public TimeOnly Hora { get; set; }
        public Especialidad Especialidad { get; set; }
        public int EspecialidadId { get; set; }
        public EstadoTurno EstadoTurno { get; set; }
        public int EstadoTurnoId { get; set; }
    }
}
