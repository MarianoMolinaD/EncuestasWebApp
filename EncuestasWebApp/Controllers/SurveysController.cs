using DAL;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public  IActionResult Create()
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

                TempData["Success"] = "Encuesta creada exitosamente.";
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                TempData["Error"] = "Error inesperado al guardar la encuesta";
                return View();
            }
        }
    }
}
