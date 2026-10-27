namespace BDA_Turnos.Models
{
    public class EspecialidadMetricasDTO
    {
        public int EspecialidadId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int MedicosActivos { get; set; }
        public int TurnosTotales { get; set; }
        public int MedicosConDisponibilidad { get; set; }
    }
}
