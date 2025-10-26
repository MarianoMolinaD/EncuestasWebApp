using Microsoft.AspNetCore.Mvc;

namespace EncuestasWebApp.Controllers
{
    public class SurveysController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
