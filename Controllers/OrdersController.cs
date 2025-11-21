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
            var orders = await _orderService.GetAllOrdersAsync(page, pageSize);
            var totalCount = await _orderService.GetOrderCountAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return View(orders ?? new List<Order>());
        }

        // GET: Sipariş detayı
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                TempData["Error"] = "Sipariş bulunamadı";
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }
    }
}