using Microsoft.AspNetCore.Mvc;
using Akasya.CRM.Core.Services;
using Akasya.CRM.Core.Entities;

namespace Akasya.CRM.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: Tüm siparişler
        public async Task<IActionResult> Index(int page = 1, int pageSize = 1000)
        {
            var orders = await _orderService.GetOrdersAsync(page, pageSize);

            // API’den toplam sayıyı almak mümkün değilse ViewBag.TotalCount ve TotalPages opsiyonel
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = orders.Count; // sadece çekilen sayfa
            ViewBag.TotalPages = 1; // Toplam sayfa sayısını API bilmediği için 1 yapıyoruz

            return View(orders ?? new List<Order>());
        }

        // GET: Sipariş detayı
        public async Task<IActionResult> Details(int id)
        {
            var orders = await _orderService.GetOrdersAsync(); // tüm siparişleri çekiyoruz
            var order = orders.FirstOrDefault(o => o.ID == id);

            if (order == null)
            {
                TempData["Error"] = "Sipariş bulunamadı";
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }
    }
}
