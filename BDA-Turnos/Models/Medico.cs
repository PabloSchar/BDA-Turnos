namespace BDA_Turnos.Models
{
    public class Medico
    {
        public Usuario Usuario { get; set; }
        public int UsuarioId { get; set; }
        public int Matricula { get; set; }
        public virtual ICollection<Especialidad> Especialidades { get; set; } = new List<Especialidad>();
    }
}
