using Microsoft.AspNetCore.Mvc;

namespace Akasya.CRM.Controllers
{
    public class CalendarController : Controller
    {
        // GET: Calendar
        public IActionResult Index()
        {
            return View();
        }
    }
}