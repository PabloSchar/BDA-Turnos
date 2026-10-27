namespace BDA_Turnos.Models
{
    public class MedicoEspecialidadDTO
    {
        public int MedicoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Matricula { get; set; }
        public int TurnosOcupados { get; set; }
        public int CuposTotales { get; set; }
        public int PorcentajeOcupacion { get; set; }
        public string EstadoSemaforo { get; set; } = string.Empty;
        public string TextoEstado { get; set; } = string.Empty;
        public string ClaseBadge { get; set; } = string.Empty;
        public string ClaseBarra { get; set; } = string.Empty;
    }
}
