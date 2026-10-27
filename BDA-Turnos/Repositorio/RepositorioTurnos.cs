using BDA_Turnos.Models;
using Microsoft.EntityFrameworkCore;

namespace BDA_Turnos.Repositorio
{
    public class RepositorioTurnos
    {
        private readonly TurnosDBContext _db;

        public RepositorioTurnos(TurnosDBContext contextoCompartido)
        {
            _db = contextoCompartido;
        }

        public async Task<Medico?> ObtenerMedicoConUsuarioAsync(int medicoId)
        {
            return await _db.Medicos
                .Include(m => m.Usuario)
                .Include(m => m.Especialidades)
                .FirstOrDefaultAsync(m => m.UsuarioId == medicoId);
        }

        public async Task<List<TurnoDTO>> ObtenerTurnosPorMedicoAsync(int medicoId, int especialidadId)
        {
            return await _db.Turnos
                .Where(t => t.MedicoId == medicoId && t.EspecialidadId == especialidadId)
                .OrderBy(t => t.Fecha)
                .ThenBy(t => t.Hora)
                .Select(t => new TurnoDTO
                {
                    TurnoId = t.TurnoId,
                    Fecha = t.Fecha,
                    Hora = t.Hora,
                    PacienteNombre = t.Paciente.Usuario.Nombre,
                    PacienteDni = t.Paciente.DNI,
                    EstadoNombre = t.EstadoTurno.Nombre,
                    EstadoTurnoId = t.EstadoTurnoId
                })
                .ToListAsync();
        }
    }
}
