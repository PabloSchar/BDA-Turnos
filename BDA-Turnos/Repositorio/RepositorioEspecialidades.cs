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

        public async Task<List<EspecialidadMetricasDTO>> ObtenerEspecialidadesConMetricasAsync()
        {
            var especialidadesBase = await _db.Especialidades
                .Select(e => new
                {
                    e.EspecialidadId,
                    e.Nombre
                })
                .ToListAsync();

            var resultado = new List<EspecialidadMetricasDTO>();

            foreach (var esp in especialidadesBase)
            {
                //Obtengo TODOS los médicos activos vinculados a esta especialidad
                var medicosIds = await _db.Medicos
                    .Where(m => m.Especialidades.Any(e => e.EspecialidadId == esp.EspecialidadId))
                    .Select(m => m.UsuarioId)
                    .ToListAsync();

                //Médicos ACTIVOS en el sistema para esta especialidad
                int medicosActivos = medicosIds.Count;

                //Turnos totales registrados en la especialidad
                int turnosTotales = await _db.Turnos
                    .CountAsync(t => t.EspecialidadId == esp.EspecialidadId);

                // la DISPONIBILIDAD (médicos activos que tienen < 8 turnos)
                int medicosConDisponibilidad = 0;

                if (medicosActivos > 0)
                {
                    //Agrupo los turnos por médico
                    var turnosPorMedico = await _db.Turnos
                        .Where(t => t.EspecialidadId == esp.EspecialidadId && medicosIds.Contains(t.MedicoId))
                        .GroupBy(t => t.MedicoId)
                        .Select(g => new { MedicoId = g.Key, Conteo = g.Count() })
                        .ToDictionaryAsync(g => g.MedicoId, g => g.Conteo);

                    foreach (var medicoId in medicosIds)
                    {
                        int turnosDelMedico = turnosPorMedico.TryGetValue(medicoId, out int conteo) ? conteo : 0;

                        if (turnosDelMedico < 8)
                        {
                            medicosConDisponibilidad++;
                        }
                    }
                }

                resultado.Add(new EspecialidadMetricasDTO
                {
                    EspecialidadId = esp.EspecialidadId,
                    Nombre = esp.Nombre,
                    MedicosActivos = medicosActivos,
                    TurnosTotales = turnosTotales,
                    MedicosConDisponibilidad = medicosConDisponibilidad 
                });
            }

            return resultado;
        }

        public async Task<Especialidad?> ObtenerEspecialidadPorIdAsync(int especialidadId)
        {
            return await _db.Especialidades.FindAsync(especialidadId);
        }

        public async Task<int> TotalEspecialidadesAsync() => await _db.Especialidades.CountAsync();
        public async Task<int> TotalMedicosAsync() => await _db.Medicos.CountAsync();
        public async Task<int> TotalTurnosAsync() => await _db.Turnos.CountAsync();
        public async Task<int> TotalCuposDisponiblesAsync() => await _db.Turnos.CountAsync(t => t.EstadoTurnoId == 1);
    }
}