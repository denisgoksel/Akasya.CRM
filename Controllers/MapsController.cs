using Microsoft.AspNetCore.Mvc;

namespace Akasya.CRM.Controllers
{
    public class MapsController : Controller
    {
        // GET: Maps
        public IActionResult Google()
        {
            return View();
        }
        public IActionResult Vector()
        {
            return View();
        }
    }
}