using DAL;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EncuestasWebApp.Controllers
{
    public class SurveysController : Controller
    {
        private readonly SurveyDAL _surveyDal;
        private readonly IConfiguration _config;

        public SurveysController(IConfiguration config)
        {
            _config = config;
            _surveyDal = new SurveyDAL(_config.GetConnectionString("DefaultConnection"));
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var Surveys = await _surveyDal.SurveyShowAsync(userId);
                return View(Surveys);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = "Ocurrio un error al obtener la información";
            }
            return View();
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(SurveyCreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                model.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                model.UniqueLink = Guid.NewGuid().ToString("N");

                int surveyId = await _surveyDal.CreateSurveyAsync(model);

                TempData["Success"] = $"Encuesta No {surveyId} creada exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error inesperado al guardar la encuesta";
                return View();
            }
        }

        [Authorize]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                int result = await _surveyDal.DeleteSurveryAsync(id);
                if (result == 0)
                    return Json(new { success = false, message = "No se pudo eleminar la encuesta" });

                return Json(new { success = true, message = "Registro eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "No se pudo eliminar la encuesta" });
            }
        }

        [Authorize]
        public async Task<IActionResult> Edit(SurveyCreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                int surveyId = await _surveyDal.CreateSurveyAsync(model);

                TempData["Success"] = $"Encuesta No {surveyId} creada exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error inesperado al guardar la encuesta";
                return View();
            }
        }

        [HttpGet("Surveys/ViewSurvey/{link}")]
        public async Task<IActionResult> ViewSurvey(string link)
        {
            try
            {
                var survey = await _surveyDal.GetSurveyWithFieldsByLinkAsync(link);

                if (survey == null)
                {
                    return NotFound("Encuesta no encontrada o eliminada.");
                }

                return View("FillSurvey", survey);
            }
            catch
            {
                return RedirectToAction("Error","Home"); 
            }    
        }

        [HttpPost]
        public async Task<IActionResult> SubmitSurvey([FromBody] SurveyResponseViewModel model)
        {
            if (model == null || model.SurveyId <= 0 || model.Answers == null || !model.Answers.Any())
            {
                return Json(new { success = false, message = "Datos inválidos." });
            }

            try
            {
                await _surveyDal.SaveSurveyResponseAsync(model);
                return Json(new { success = true, message = "Respuestas guardadas correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al guardar las respuestas." });
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var survey = await _surveyDal.GetSurveyWithFieldsAsync(id);
            if (survey == null)
            {
                TempData["Error"] = "Encuesta no encontrada.";
                return RedirectToAction("Index");
            }
            return View(survey);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(SurveyEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Datos inválidos.";
                return View(model);
            }

            try
            {
                await _surveyDal.UpdateSurveyAsync(model);
                TempData["Success"] = "Encuesta actualizada correctamente.";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Error"] = "Error al actualizar la encuesta.";
                return View(model);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Results(int id)
        {
            try
            {
                var results = await _surveyDal.GetSurveyResultsAsync(id);

                if (results == null)
                {
                    TempData["Error"] = "No se encontraron resultados para esta encuesta.";
                    return RedirectToAction("Index");

                }

                return View(results);

            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error inesperado al buscar la encuesta.";
                return RedirectToAction("Index");
            }
        }

    }
}
