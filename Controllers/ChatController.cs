using Microsoft.AspNetCore.Mvc;

namespace Akasya.CRM.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat
        public IActionResult Index()
        {
            return View();
        }
    }
}