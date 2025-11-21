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
// Database Configuration
// ---------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Akasya.CRM.Infrastructure")
    )
    //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) // BU SATIRI EKLEYİN
);

// ---------------------------
// CORE SERVICES
// ---------------------------
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<OrderService>();

// ---------------------------
// CACHE SERVICES
// ---------------------------
builder.Services.AddScoped<ICacheService, CustomerCacheService>();
builder.Services.AddScoped<ICacheService, OrderCacheService>();
builder.Services.AddScoped<CacheUpdateManager>();

// ---------------------------
// HTTP CLIENT CONFIGURATIONS
// ---------------------------

// AuthService HttpClient
builder.Services.AddHttpClient<AuthService>(client =>
{
    client.BaseAddress = new Uri("http://185.248.59.144:8084");
    client.DefaultRequestHeaders.Add("APIKey", "4kGHIj0aDNREpPD1Vmg84z9vEi63zkkipmsxukZUW0FmUot1e8p2aY1TdYLr4S0pxoVVCboVUN4ol/SwZKSWHnhlYlV2riD32qcidZR0sXk=");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// CustomerService HttpClient
builder.Services.AddHttpClient<CustomerService>(client =>
{
    client.BaseAddress = new Uri("http://185.248.59.144:8084");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// OrderService HttpClient  
builder.Services.AddHttpClient<OrderService>(client =>
{
    client.BaseAddress = new Uri("http://185.248.59.144:8084");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ---------------------------
// SESSION CONFIGURATION
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
// AUTHENTICATION CONFIGURATION
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
// AUTHORIZATION POLICIES
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
// DATABASE MIGRATION (Development Only)
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
// CONFIGURE HTTP PIPELINE
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

// Controller Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Fallback route - Auth controller'a yönlendir
app.MapFallbackToController("Login", "Auth");

app.Run();