using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using InovaSaude.Blazor.Data;
using InovaSaude.Blazor.Services;

// Configuração global do Npgsql para aceitar DateTime sem Kind
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Session Configuration
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Rate Limiting Configuration
builder.Services.AddMemoryCache();
builder.Services.Configure<AspNetCoreRateLimit.IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429;
    options.RealIpHeader = "X-Real-IP";
    options.GeneralRules = new List<AspNetCoreRateLimit.RateLimitRule>
    {
        new AspNetCoreRateLimit.RateLimitRule
        {
            Endpoint = "*/login",
            Period = "1m",
            Limit = 5 // 5 tentativas por minuto
        },
        new AspNetCoreRateLimit.RateLimitRule
        {
            Endpoint = "*",
            Period = "1m",
            Limit = 100 // 100 requisições por minuto
        }
    };
});
builder.Services.AddSingleton<AspNetCoreRateLimit.IIpPolicyStore, AspNetCoreRateLimit.MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<AspNetCoreRateLimit.IRateLimitCounterStore, AspNetCoreRateLimit.MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<AspNetCoreRateLimit.IRateLimitConfiguration, AspNetCoreRateLimit.RateLimitConfiguration>();
builder.Services.AddSingleton<AspNetCoreRateLimit.IProcessingStrategy, AspNetCoreRateLimit.AsyncKeyLockProcessingStrategy>();

// Configurar Data Protection baseado no ambiente
if (builder.Environment.IsProduction())
{
    // Em produção (Render), usar sistema de arquivos persistente
    var dataProtectionPath = Path.Combine("/app", "keys");
    try
    {
        if (!Directory.Exists(dataProtectionPath))
        {
            Directory.CreateDirectory(dataProtectionPath);
        }

        Console.WriteLine($"[DataProtection] Configured to use filesystem at {dataProtectionPath}");
        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
            .SetApplicationName("InovaSaude")
            .SetDefaultKeyLifetime(TimeSpan.FromDays(90));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DataProtection] WARNING: Could not create persistent storage: {ex.Message}");
        Console.WriteLine("[DataProtection] Falling back to ephemeral provider");
        builder.Services.AddDataProtection()
            .SetApplicationName("InovaSaude")
            .SetDefaultKeyLifetime(TimeSpan.FromDays(90));
    }
}
else
{
    // Em desenvolvimento, usar filesystem local
    var dataProtectionPath = Path.Combine(Directory.GetCurrentDirectory(), "keys");
    if (!Directory.Exists(dataProtectionPath))
    {
  Directory.CreateDirectory(dataProtectionPath);
    }

    builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
        .SetApplicationName("InovaSaude")
        .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

    Console.WriteLine($"[DataProtection] Configured to use filesystem at {dataProtectionPath}");
}

// Configuração de Antiforgery para funcionar com proxy reverso (Render)
builder.Services.AddAntiforgery(options =>
{
// O proxy do Render lida com SSL externamente, internamente é HTTP
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.HttpOnly = true;
    options.Cookie.Name = ".InovaSaude.Antiforgery";
});

// Função para converter DATABASE_URL (formato postgres://) para connection string Npgsql
static string ConvertPostgresUrlToConnectionString(string databaseUrl)
{
    // Formato esperado: postgres://user:password@host:port/database
    // ou postgresql://user:password@host:port/database
    if (string.IsNullOrEmpty(databaseUrl))
        return string.Empty;

    // Se já está no formato de connection string ADO.NET, retornar como está
    if (databaseUrl.Contains("Host=") || databaseUrl.Contains("Server="))
        return databaseUrl;

    try
    {
        // Substituir postgres:// ou postgresql:// por scheme válido para Uri
        var url = databaseUrl.Replace("postgres://", "http://").Replace("postgresql://", "http://");
        var uri = new Uri(url);

        var userInfo = uri.UserInfo.Split(':');
        var username = userInfo.Length > 0 ? userInfo[0] : "";
        var password = userInfo.Length > 1 ? userInfo[1] : "";
        var host = uri.Host;
        
        // FIX: Uri.Port retorna -1 quando não há porta especificada, usar 5432 como padrão
        var port = (uri.Port > 0 && uri.Port != 80) ? uri.Port : 5432;
        
        var database = uri.AbsolutePath.TrimStart('/');

        // Construir connection string no formato Npgsql
        var connString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";

// Log para debug (será visível nos logs do Render)
        Console.WriteLine($"[DB] Converted DATABASE_URL to connection string:");
        Console.WriteLine($"[DB] Host={host}, Port={port}, Database={database}, Username={username}");

        return connString;
    }
    catch (Exception ex)
 {
   Console.WriteLine($"[DB] Error converting DATABASE_URL: {ex.Message}");
        // Se falhar o parse, retornar a string original
   return databaseUrl;
    }
}

// Add Entity Framework Core (SQL Server local, PostgreSQL production)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

// Log para debug
Console.WriteLine($"[DB] Environment.IsProduction: {builder.Environment.IsProduction()}");
Console.WriteLine($"[DB] DATABASE_URL exists: {!string.IsNullOrEmpty(databaseUrl)}");

// Determinar qual connection string usar
string? finalConnectionString = null;

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Se DATABASE_URL está definida (produção no Render), converter para formato Npgsql
    Console.WriteLine($"[DB] Using DATABASE_URL from environment");
    finalConnectionString = ConvertPostgresUrlToConnectionString(databaseUrl);
}
else if (!string.IsNullOrEmpty(connectionString))
{
    // Usar connection string do appsettings
    Console.WriteLine($"[DB] Using ConnectionString from appsettings");
    finalConnectionString = connectionString;
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Se está em produção OU se a connection string contém "Host=" (PostgreSQL)
    if (builder.Environment.IsProduction() || finalConnectionString?.Contains("Host=") == true)
    {
  // PostgreSQL para produção (Render)
        if (string.IsNullOrEmpty(finalConnectionString))
   {
            throw new InvalidOperationException("PostgreSQL connection string not found. Set DATABASE_URL or ConnectionStrings__DefaultConnection");
     }

    Console.WriteLine("[DB] Configuring PostgreSQL with Npgsql");
    options.UseNpgsql(finalConnectionString);
    }
    else
    {
   // SQL Server para desenvolvimento local
 Console.WriteLine("[DB] Configuring SQL Server");
        options.UseSqlServer(finalConnectionString ?? throw new InvalidOperationException("Connection string not found"));
    }
});

// Authentication & Authorization (basic cookie-based)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

// Policies de Autorização por Perfil
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireClaim("Perfil", "ADMIN"));

    options.AddPolicy("GestorOuAdmin", policy => 
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "Perfil" && 
                (c.Value == "ADMIN" || c.Value == "GESTOR"))));

    options.AddPolicy("CoordenadorOuSuperior", policy => 
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "Perfil" && 
                (c.Value == "ADMIN" || c.Value == "GESTOR" || c.Value == "COORDENADOR"))));

    options.AddPolicy("TodosAutenticados", policy => 
        policy.RequireAuthenticatedUser());
});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        if (builder.Environment.IsProduction())
        {
            policy.WithOrigins("https://inovasaude-blazor.onrender.com")
                  .AllowCredentials()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

builder.Services.AddAuthorization();

// Add custom services
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ToastService>(); // TOAST SERVICE CRÍTICO!
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ESFService>();
builder.Services.AddScoped<FuncionarioService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<DespesaService>();
builder.Services.AddScoped<RelatorioService>();
builder.Services.AddScoped<PdfService>();
builder.Services.AddScoped<WorkflowService>();
builder.Services.AddScoped<IntegrationService>();
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<ImportacaoService>();
builder.Services.AddScoped<WebhookService>();

// APIs Externas (e-SUS PEC, NEMESIS)
builder.Services.AddHttpClient();
builder.Services.AddScoped<InovaSaude.Blazor.Services.Integrations.EsusPecIntegrationService>();
builder.Services.AddScoped<InovaSaude.Blazor.Services.Integrations.NemesisIntegrationService>();

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
    // HSTS em produção
    app.UseHsts();
}

// Headers de Segurança
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add("Content-Security-Policy", 
        "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:;");
    await next();
});

// HTTPS Redirect (exceto no Render que já usa proxy)
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

// Rate Limiting Middleware
app.UseMiddleware<AspNetCoreRateLimit.IpRateLimitMiddleware>();

// Session
app.UseSession();

// Audit Middleware
app.UseMiddleware<InovaSaude.Blazor.Middleware.AuditMiddleware>();

app.UseRouting();

// CORS
app.UseCors("Production");

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

// Aplicar migrations automaticamente em produção
if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Aplicando migrations automaticamente...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Migrations aplicadas com sucesso!");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao aplicar migrations automaticamente.");
    }
}

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
// Map controller routes (AccountController)
app.MapControllers();

app.Run();
