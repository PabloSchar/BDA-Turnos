using BDA_Turnos.Models;
using Microsoft.EntityFrameworkCore;

namespace BDA_Turnos.Repositorio
{
    public class RepositorioMedicos
    {
        private readonly TurnosDBContext _db;

        public RepositorioMedicos(TurnosDBContext contextoCompartido)
        {
            _db = contextoCompartido;
        }

        public async Task<List<MedicoEspecialidadDTO>> ObtenerMedicosPorEspecialidadAsync(int especialidadId)
        {
            const int cupoMaximoEstandard = 10;

            //Carga los médicos
            List<MedicoBaseDTO> medicosBase = await _db.Medicos
                .Where(m => m.Especialidades.Any(e => e.EspecialidadId == especialidadId))
                .Select(m => new MedicoBaseDTO
                {
                    MedicoId = m.UsuarioId,
                    Nombre = m.Usuario.Nombre,
                    Matricula = m.Matricula
                })
                .ToListAsync();

            if (!medicosBase.Any()) return new List<MedicoEspecialidadDTO>();

            var idsMedicos = medicosBase.Select(m => m.MedicoId).ToList();

            //Traigo los conteos de turnos mapeados
            List<ConteoTurnosDTO> turnosConteos = await _db.Turnos
                .Where(t => t.EspecialidadId == especialidadId && idsMedicos.Contains(t.MedicoId))
                .GroupBy(t => t.MedicoId)
                .Select(g => new ConteoTurnosDTO { MedicoId = g.Key, Conteo = g.Count() })
                .ToListAsync();

            var diccTurnos = turnosConteos.ToDictionary(k => k.MedicoId, v => v.Conteo);

            var resultado = new List<MedicoEspecialidadDTO>();

            foreach (var m in medicosBase)
            {
                int turnosOcupados = diccTurnos.TryGetValue(m.MedicoId, out int conteo) ? conteo : 0;
                int porcentaje = (int)Math.Min(100, Math.Round((double)turnosOcupados / cupoMaximoEstandard * 100));

                resultado.Add(new MedicoEspecialidadDTO
                {
                    MedicoId = m.MedicoId,
                    Nombre = m.Nombre,
                    Matricula = m.Matricula,
                    TurnosOcupados = turnosOcupados,
                    CuposTotales = cupoMaximoEstandard,
                    PorcentajeOcupacion = porcentaje,
                    EstadoSemaforo = turnosOcupados >= 8 ? "Rojo" : turnosOcupados >= 4 ? "Amarillo" : "Verde",
                    TextoEstado = turnosOcupados >= 8 ? "Crítica" : turnosOcupados >= 4 ? "Advertencia" : "Normal",
                    ClaseBadge = turnosOcupados >= 8 ? "b-red" : turnosOcupados >= 4 ? "b-amber" : "b-green",
                    ClaseBarra = turnosOcupados >= 8 ? "r" : turnosOcupados >= 4 ? "a" : "g"
                });
            }

            return resultado;
        }
    }
}
