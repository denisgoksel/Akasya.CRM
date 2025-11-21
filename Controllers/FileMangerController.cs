using Microsoft.AspNetCore.Mvc;

namespace Akasya.CRM.Controllers
{
    public class FileMangerController : Controller
    {
        // GET: FileManger
        public IActionResult Index()
        {
            return View();
        }
    }
}