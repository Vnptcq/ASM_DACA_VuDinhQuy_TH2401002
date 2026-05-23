using HR_Management.Application;
using HR_Management.Infrastructure;
using HR_Management.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddInfrastructure(connectionString);
builder.Services.AddApplication();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

await ApplyMigrationsAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

var urls = builder.Configuration["ASPNETCORE_URLS"] ?? string.Empty;
if (urls.Contains("https", StringComparison.OrdinalIgnoreCase))
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

static async Task ApplyMigrationsAsync(IServiceProvider services)
{
    const int maxAttempts = 15;
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync();
            return;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            Console.WriteLine($"Database not ready (attempt {attempt}/{maxAttempts}): {ex.Message}");
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }

    throw new InvalidOperationException("Could not apply database migrations after multiple attempts.");
}
