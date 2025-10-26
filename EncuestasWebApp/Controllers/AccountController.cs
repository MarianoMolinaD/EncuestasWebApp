using DAL;
using EncuestasWebApp.Services;
using Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace EncuestasWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserDAL _userDal;
        private readonly IPasswordHasher _hasher;
        private readonly IConfiguration _config;

        public AccountController(IPasswordHasher hasher, IConfiguration config)
        {
            _hasher = hasher;
            _config = config;
            _userDal = new UserDAL(_config.GetConnectionString("DefaultConnection"));
        }
        public IActionResult Index() => View();

        public async Task<IActionResult> Login(string userName, string password)
        {
            try
            {
                var user = await _userDal.GetByUserNameAsync(userName);

                if (user == null || !_hasher.Verify(password, user.PasswordHash))
                {
                    TempData["Error"] = "Nombre de usuario o contraseña incorrecto.";
                    return RedirectToAction("Index");
                }

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, "Admin")
            };

                var identity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("CookieAuth", principal, new AuthenticationProperties
                {
                    IsPersistent = true
                });

                return RedirectToAction("Index", "Surveys");
            }
            catch (SqlException slqEx)
            {
                TempData["Error"] = "Error en la base de datos. COntacta con soporte técnico";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Se produjo un error inesperado. Intentalo mas tarde";
                return RedirectToAction("Index");
            }
        }
    }
}
