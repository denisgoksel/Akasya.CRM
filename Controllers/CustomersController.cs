using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Akasya.CRM.Infrastructure.Data;
using Akasya.CRM.Core.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

public class CustomersController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public CustomersController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            // ✅ DOĞRUDAN LOCAL DB'DEN ÇEK
            var customers = await _dbContext.Customers
                .AsNoTracking() // ✅ Performance için
                .OrderBy(c => c.CODE)
                .ToListAsync();

            return View(customers);
        }
        catch (Exception ex)
        {
            // Hata durumunda boş liste dön
            return View(new List<Customer>());
        }
    }
}