using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BDA_Turnos.Repositorio;

namespace BDA_Turnos.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly RepositorioEspecialidades _repoEspecialidades;

        public DashboardController(RepositorioEspecialidades repoEspecialidades)
        {
            _repoEspecialidades = repoEspecialidades;
        }

        public async Task<IActionResult> Especialidades()
        {
            var especialidades = await _repoEspecialidades.ObtenerEspecialidadesConMetricasAsync();

            ViewBag.TotalEspecialidades = await _repoEspecialidades.TotalEspecialidadesAsync();
            ViewBag.TotalMedicos = await _repoEspecialidades.TotalMedicosAsync();
            ViewBag.TotalTurnos = await _repoEspecialidades.TotalTurnosAsync();
            ViewBag.TotalCupos = await _repoEspecialidades.TotalCuposDisponiblesAsync();

            return View(especialidades);
        }
    }
}