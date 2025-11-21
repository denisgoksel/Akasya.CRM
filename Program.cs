
using Akasya.CRM.Core.Services;
using Akasya.CRM.Infrastructure.Data;
using Akasya.CRM.Infrastructure.Interfaces;
using Akasya.CRM.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ---------------------------
// MEVCUT SERVİSLER (DEĞİŞTİRMEYİN)
// ---------------------------
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICacheService, OrderCacheService>();
builder.Services.AddHttpClient<IOrderService, OrderService>();
// ---------------------------
// ✅ YENİ: CACHE SERVİSLERİ EKLENDİ
// ---------------------------
builder.Services.AddSingleton<ICacheService, CustomerCacheService>();
builder.Services.AddScoped<CacheUpdateManager>();

// ---------------------------
// Database Configuration (MEVCUT - DEĞİŞTİRMEYİN)
// ---------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Akasya.CRM.Infrastructure")
    ));

// ---------------------------
// HttpClient Configuration (MEVCUT - DEĞİŞTİRMEYİN)
// ---------------------------
builder.Services.AddHttpClient<AuthService>(client =>
{
    client.BaseAddress = new Uri("http://185.248.59.144:8084");
    client.DefaultRequestHeaders.Add("APIKey", "4kGHIj0aDNREpPD1Vmg84z9vEi63zkkipmsxukZUW0FmUot1e8p2aY1TdYLr4S0pxoVVCboVUN4ol/SwZKSWHnhlYlV2riD32qcidZR0sXk=");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ✅ CUSTOMER SERVICE HTTP CLIENT EKLENDİ
builder.Services.AddHttpClient<CustomerService>(client =>
{
    client.BaseAddress = new Uri("http://185.248.59.144:8084");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ✅ ORDER SERVICE HTTP CLIENT EKLENDİ  
builder.Services.AddHttpClient<OrderService>(client =>
{
    client.BaseAddress = new Uri("http://185.248.59.144:8084");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ---------------------------
// Session Configuration (MEVCUT - DEĞİŞTİRMEYİN)
// ---------------------------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// ---------------------------
// Authentication Configuration (MEVCUT - DEĞİŞTİRMEYİN)
// ---------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.LogoutPath = "/Auth/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

// ---------------------------
// Authorization Policies (MEVCUT - DEĞİŞTİRMEYİN)
// ---------------------------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("Role", "Admin"));

    options.AddPolicy("UserOnly", policy =>
        policy.RequireClaim("Role", "User", "Admin"));
});

var app = builder.Build();

// ---------------------------
// Database Migration (Development Only) (MEVCUT - DEĞİŞTİRMEYİN)
// ---------------------------
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            // Otomatik migration uygula
            dbContext.Database.Migrate();
            Console.WriteLine("Database migrated successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database migration failed: {ex.Message}");
        }
    }
}

// ---------------------------
// Configure HTTP Pipeline (MEVCUT - DEĞİŞTİRMEYİN)
// ---------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Controller Routes (MEVCUT - DEĞİŞTİRMEYİN)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Fallback route - Auth controller'a yönlendir (MEVCUT - DEĞİŞTİRMEYİN)
app.MapFallbackToController("Login", "Auth");

app.Run();