namespace BDA_Turnos.Models
{
    public class TurnoDTO
    {
        public int TurnoId { get; set; }
        public DateOnly Fecha { get; set; }
        public TimeOnly Hora { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public int PacienteDni { get; set; }
        public string EstadoNombre { get; set; } = string.Empty;
        public int EstadoTurnoId { get; set; }
        public string FechaFormateada => Fecha.ToDateTime(TimeOnly.MinValue).ToString("dd MMM yyyy");
        public string DiaNombre => Fecha.ToDateTime(TimeOnly.MinValue).ToString("dddd");
        public string HoraFormateada => Hora.ToString(@"hh\:mm");
        public string DniFormateado => PacienteDni.ToString("N0");

        public string ClaseBadge => EstadoNombre.ToLower() switch
        {
            var e when e.Contains("atend") => "b-green",
            var e when e.Contains("pend") => "b-amber",
            var e when e.Contains("confir") => "b-blue",
            var e when e.Contains("canc") => "b-red",
            _ => "b-gray"
        };
    }
}
