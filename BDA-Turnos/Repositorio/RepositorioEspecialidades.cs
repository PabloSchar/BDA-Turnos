using BDA_Turnos.Models;
using Microsoft.EntityFrameworkCore;

namespace BDA_Turnos.Repositorio
{
    public class RepositorioEspecialidades
    {
        private readonly TurnosDBContext _db;

        public RepositorioEspecialidades(TurnosDBContext contextoCompartido)
        {
            _db = contextoCompartido;
        }

        public async Task<List<dynamic>> ObtenerEspecialidadesConMetricasAsync()
        {
            var especialidades = await _db.Especialidades
                .Select(e => new
                {
                    e.EspecialidadId,
                    e.Nombre,
                    MedicosActivos = _db.Medicos.Count(m => m.Especialidades.Any(esp => esp.EspecialidadId == e.EspecialidadId)),
                    TurnosTotales = _db.Turnos.Count(t => t.EspecialidadId == e.EspecialidadId),
                    MedicosConDisponibilidad = _db.Medicos.Where(m => m.Especialidades.Any(esp => esp.EspecialidadId == e.EspecialidadId)).Count(m => _db.Turnos.Count(t => t.MedicoId == m.UsuarioId && t.EspecialidadId == e.EspecialidadId) < 8)
                })
                .ToListAsync();

            return especialidades.Cast<dynamic>().ToList();
        }

        public async Task<int> TotalEspecialidadesAsync() => await _db.Especialidades.CountAsync();
        public async Task<int> TotalMedicosAsync() => await _db.Medicos.CountAsync();
        public async Task<int> TotalTurnosAsync() => await _db.Turnos.CountAsync();
        public async Task<int> TotalCuposDisponiblesAsync() => await _db.Turnos.CountAsync(t => t.EstadoTurnoId == 1);
    }
}