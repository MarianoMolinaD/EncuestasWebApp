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
    }
}
