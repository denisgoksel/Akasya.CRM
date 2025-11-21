using Akasya.CRM.Core.Services;
using Akasya.CRM.Infrastructure.Interfaces;
using Akasya.CRM.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Akasya.CRM.Web.Controllers
{
    //[Authorize]
    public class CacheUpdateController : Controller
    {
        private readonly CacheUpdateManager _cacheUpdateManager;
        private readonly ILogger<CacheUpdateController> _logger;
        private readonly IServiceProvider _services;
        public CacheUpdateController(CacheUpdateManager cacheUpdateManager, ILogger<CacheUpdateController> logger, IServiceProvider services    )
        {
            _cacheUpdateManager = cacheUpdateManager;
            _logger = logger;
            _services = services;
        }

        [HttpGet]
        //[Authorize(Policy = "UserOnly")]
        public IActionResult Index()
        {
            var serviceNames = _cacheUpdateManager.GetServiceNames();
            var lastResults = _cacheUpdateManager.GetLastResults();

            var model = new CacheUpdateViewModel
            {
                Services = serviceNames,
                LastResults = lastResults,
                LastUpdated = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        //[Authorize(Policy = "AdminOnly")]
        public async Task<JsonResult> UpdateAll()
        {
            try
            {
                _logger.LogInformation("Tüm cache güncellemeleri MVC üzerinden tetiklendi. Kullanıcı: {User}", User.Identity.Name);

                var result = await _cacheUpdateManager.UpdateAllAsync();

                return Json(new
                {
                    success = true,
                    message = $"Tüm cache güncellemeleri tamamlandı! {result.SuccessfulServices}/{result.Services.Count} başarılı, {result.TotalRecordsProcessed} kayıt işlendi.",
                    logs = result.AllLogs
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache güncelleme hatası");
                return Json(new
                {
                    success = false,
                    message = $"Cache güncelleme hatası: {ex.Message}",
                    logs = new List<string> { $"❌ Hata: {ex.Message}" }
                });
            }
        }

        [HttpPost]
       // [Authorize(Policy = "UserOnly")]
        public async Task<JsonResult> UpdateSingle(string serviceName)
        {
            try
            {
                _logger.LogInformation("{ServiceName} cache güncellemesi MVC üzerinden tetiklendi. Kullanıcı: {User}", serviceName, User.Identity.Name);

                var result = await _cacheUpdateManager.UpdateSingleAsync(serviceName);

                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    logs = result.Logs
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ServiceName} cache güncelleme hatası", serviceName);
                return Json(new
                {
                    success = false,
                    message = $"{serviceName} cache güncelleme hatası: {ex.Message}",
                    logs = new List<string> { $"❌ Hata: {ex.Message}" }
                });
            }
        }

        [HttpGet]
        public JsonResult GetStatus()
        {
            var lastResults = _cacheUpdateManager.GetLastResults();
            var serviceNames = _cacheUpdateManager.GetServiceNames();

            return Json(new
            {
                services = serviceNames,
                lastResults = lastResults,
                lastUpdated = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")
            });
        }
        [HttpPost]
        public async Task<JsonResult> TestApiConnection()
        {
            try
            {
                using var scope = _services.CreateScope();
                var customerService = scope.ServiceProvider.GetRequiredService<CustomerService>();

                Console.WriteLine("🧪 Test API bağlantısı başlatılıyor...");

                var customers = await customerService.GetCustomersAsync(1, 10);

                return Json(new
                {
                    success = true,
                    message = $"API testi tamamlandı. {customers?.Count ?? 0} müşteri çekildi.",
                    customerCount = customers?.Count ?? 0,
                    firstCustomer = customers?.FirstOrDefault()?.CODE ?? "Yok"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test API hatası: {ex.Message}");
                return Json(new
                {
                    success = false,
                    message = $"API testi başarısız: {ex.Message}",
                    customerCount = 0
                });
            }
        }
        [HttpPost]
        public async Task<JsonResult> TestOrderApiConnection()
        {
            try
            {
                using var scope = _services.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<OrderService>();

                Console.WriteLine("🧪 Order API bağlantı testi başlatılıyor...");

                var result = await orderService.TestApiConnectionAsync();

                return Json(new
                {
                    success = result,
                    message = result ? "Order API bağlantısı başarılı!" : "Order API bağlantısı başarısız!",
                    testType = "Order API"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Order API test hatası: {ex.Message}");
                return Json(new
                {
                    success = false,
                    message = $"Order API test hatası: {ex.Message}",
                    testType = "Order API"
                });
            }
        }
    }
    
    public class CacheUpdateViewModel
    {
        public List<string> Services { get; set; } = new List<string>();
        public Dictionary<string, CacheUpdateResult> LastResults { get; set; } = new Dictionary<string, CacheUpdateResult>();
        public DateTime LastUpdated { get; set; }
    }
}