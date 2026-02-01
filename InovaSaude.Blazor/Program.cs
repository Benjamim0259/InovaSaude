using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Persist Data Protection keys to avoid antiforgery errors on container restart
var dataProtectionPath = Path.Combine("/app", "keys");
if (!Directory.Exists(dataProtectionPath))
{
    Directory.CreateDirectory(dataProtectionPath);
}
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetApplicationName("InovaSaude");

// Add Entity Framework Core (SQL Server local, PostgreSQL production)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsProduction() && connectionString?.Contains("Host=") == true)
    {
        // PostgreSQL para produção (Render)
        options.UseNpgsql(connectionString);
    }
    else
    {
        // SQL Server para desenvolvimento local
        options.UseSqlServer(connectionString);
    }
});

// Authentication & Authorization (basic cookie-based)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddAuthorization();

// Add custom services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<UBSService>();
builder.Services.AddScoped<DespesaService>();
builder.Services.AddScoped<RelatorioService>();
builder.Services.AddScoped<WorkflowService>();
builder.Services.AddScoped<IntegrationService>();
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<ImportacaoService>();
builder.Services.AddScoped<WebhookService>();

// Add HttpClient for API calls with BaseAddress
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(sp.GetRequiredService<NavigationManager>().BaseUri) 
});

// Add MVC controllers for login endpoints
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Ensure DB is seeded on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.InitializeAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
// Map controller routes (AccountController)
app.MapControllers();

app.Run();
