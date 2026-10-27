using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BDA_Turnos.Repositorio;

namespace BDA_Turnos.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly RepositorioEspecialidades _repoEspecialidades;
        private readonly RepositorioMedicos _repoMedicos;
        private readonly RepositorioTurnos _repoTurnos;

        public DashboardController(RepositorioEspecialidades repoEspecialidades, RepositorioMedicos repoMedicos, RepositorioTurnos repoTurnos)
        {
            _repoEspecialidades = repoEspecialidades;
            _repoMedicos = repoMedicos;
            _repoTurnos = repoTurnos;
        }

        //Especialidades

        public async Task<IActionResult> Especialidades()
        {
            var especialidades = await _repoEspecialidades.ObtenerEspecialidadesConMetricasAsync();

            ViewBag.TotalEspecialidades = await _repoEspecialidades.TotalEspecialidadesAsync();
            ViewBag.TotalMedicos = await _repoEspecialidades.TotalMedicosAsync();
            ViewBag.TotalTurnos = await _repoEspecialidades.TotalTurnosAsync();
            ViewBag.TotalCupos = await _repoEspecialidades.TotalCuposDisponiblesAsync();

            return View(especialidades);
        }

        //Medicos
        public async Task<IActionResult> Medicos(int especialidadId)
        {
            var especialidad = await _repoEspecialidades.ObtenerEspecialidadPorIdAsync(especialidadId);

            if (especialidad == null)
            {
                return RedirectToAction("Especialidades");
            }

            var medicos = await _repoMedicos.ObtenerMedicosPorEspecialidadAsync(especialidadId);

            // Datos para el encabezado y KPIs de la especialidad
            ViewBag.EspecialidadId = especialidad.EspecialidadId;
            ViewBag.NombreEspecialidad = especialidad.Nombre;
            ViewBag.TotalMedicosAsignados = medicos.Count;
            ViewBag.TotalTurnosOcupados = medicos.Sum(m => (int)m.TurnosOcupados);
            ViewBag.TotalCupos = medicos.Sum(m => (int)m.CuposTotales);

            int totalCupos = ViewBag.TotalCupos > 0 ? ViewBag.TotalCupos : 1;
            ViewBag.OcupacionGlobal = (int)Math.Round((double)ViewBag.TotalTurnosOcupados / totalCupos * 100);
            ViewBag.CuposDisponibles = Math.Max(0, ViewBag.TotalCupos - ViewBag.TotalTurnosOcupados);

            //semáforos
            ViewBag.CountVerde = medicos.Count(m => m.EstadoSemaforo == "Verde");
            ViewBag.CountAmarillo = medicos.Count(m => m.EstadoSemaforo == "Amarillo");
            ViewBag.CountRojo = medicos.Count(m => m.EstadoSemaforo == "Rojo");

            return View(medicos);
        }

        //Turnos
        public async Task<IActionResult> TurnosMedico(int medicoId, int especialidadId)
        {
            var medico = await _repoTurnos.ObtenerMedicoConUsuarioAsync(medicoId);
            var especialidad = await _repoEspecialidades.ObtenerEspecialidadPorIdAsync(especialidadId);

            if (medico == null || especialidad == null)
            {
                return RedirectToAction("Especialidades");
            }

            var turnos = await _repoTurnos.ObtenerTurnosPorMedicoAsync(medicoId, especialidadId);

            //información de cabecera
            ViewBag.MedicoId = medico.UsuarioId;
            ViewBag.NombreMedico = medico.Usuario.Nombre;
            ViewBag.Matricula = medico.Matricula;
            ViewBag.EspecialidadId = especialidad.EspecialidadId;
            ViewBag.NombreEspecialidad = especialidad.Nombre;

            // KPIs del período
            int totalTurnos = turnos.Count;
            ViewBag.TotalTurnos = totalTurnos;
            ViewBag.Pendientes = turnos.Count(t => ((string)t.EstadoNombre).ToLower().Contains("pend"));
            ViewBag.Atendidos = turnos.Count(t => ((string)t.EstadoNombre).ToLower().Contains("atend"));
            ViewBag.Confirmados = turnos.Count(t => ((string)t.EstadoNombre).ToLower().Contains("confir"));
            ViewBag.Cancelados = turnos.Count(t => ((string)t.EstadoNombre).ToLower().Contains("canc"));

            //Porcentaje de ocupación sobre cupo de 10 turnos
            int cupoEstandard = 10;
            ViewBag.PorcentajeOcupacion = (int)Math.Min(100, Math.Round((double)totalTurnos / cupoEstandard * 100));
            ViewBag.ClaseBadgeOcupacion = totalTurnos >= 8 ? "b-red" : totalTurnos >= 4 ? "b-amber" : "b-green";
            ViewBag.TextoOcupacion = totalTurnos >= 8 ? "Crítica" : totalTurnos >= 4 ? "Advertencia" : "Normal";

            return View(turnos);
        }
    }
}