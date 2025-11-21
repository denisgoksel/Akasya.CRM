using Akasya.CRM.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Akasya.CRM.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }
        public IActionResult Lockscreen()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string kullaniciKodu, string sifre)
        {
            if (string.IsNullOrEmpty(kullaniciKodu) || string.IsNullOrEmpty(sifre))
            {
                ViewBag.Error = "Kullanıcı adı ve şifre zorunlu!";
                return View();
            }

            var success = await _authService.LoginAsync(kullaniciKodu, sifre);

            if (success)
            {
                // Kullanıcıyı cookie ile authenticate et
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, kullaniciKodu),
                    // Role eklemek istersen: new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // Remember me için
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

                // Başarılı login -> Dashboard yönlendir
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ViewBag.Error = "❌ Login başarısız!";
                return View();
            }
        }
        public IActionResult Recoverpw()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
    }
}