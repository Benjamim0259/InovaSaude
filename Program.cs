```csharp
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
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

// Função para converter DATABASE_URL (formato postgres://) para connection string Npgsql
static string ConvertPostgresUrlToConnectionString(string databaseUrl)
{
    // Formato esperado: postgres://user:password@host:port/database
    // ou postgresql://user:password@host:port/database
    if (string.IsNullOrEmpty(databaseUrl))
  return string.Empty;

    try
    {
      // Substituir postgres:// ou postgresql:// por scheme válido para Uri
      var url = databaseUrl.Replace("postgres://", "http://").Replace("postgresql://", "http://");
        var uri = new Uri(url);
        
        var userInfo = uri.UserInfo.Split(':');
  var username = userInfo.Length > 0 ? userInfo[0] : "";
        var password = userInfo.Length > 1 ? userInfo[1] : "";
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 5432;
        var database = uri.AbsolutePath.TrimStart('/');
        
   // Construir connection string no formato Npgsql
    var connString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
        
        return connString;
    }
    catch
    {
        // Se falhar o parse, retornar a string original (pode já estar no formato correto)
        return databaseUrl;
    }
}

// Determinar qual connection string usar
string? finalConnectionString = null;

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Se DATABASE_URL está definida (produção no Render), converter para formato Npgsql
    finalConnectionString = ConvertPostgresUrlToConnectionString(databaseUrl);
}
else if (!string.IsNullOrEmpty(connectionString))
{
  // Usar connection string do appsettings
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
    options.UseNpgsql(finalConnectionString);
    }
    else
    {
   // SQL Server para desenvolvimento local
     options.UseSqlServer(finalConnectionString ?? throw new InvalidOperationException("Connection string not found"));
    }
});

// ...existing code...
```
