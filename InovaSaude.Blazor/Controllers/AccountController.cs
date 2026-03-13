using InovaSaude.Blazor.Models;
using InovaSaude.Blazor.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InovaSaude.Blazor.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly UsuarioService _usuarioService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(UsuarioService usuarioService, ILogger<AccountController> logger)
    {
        _usuarioService = usuarioService;
        _logger = logger;
    }

    [HttpPost("login")]
    [HttpGet("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest? bodyModel, [FromQuery] string? email, [FromQuery] string? password)
    {
        var model = bodyModel ?? new LoginRequest { Email = email ?? "", Password = password ?? "" };

        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            return BadRequest(new { message = "Email and password required" });

        var user = await _usuarioService.GetUsuarioByEmailAsync(model.Email);
        if (user == null)
        {
            if (Request.Method == "GET") return Redirect("/login?error=invalid");
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var verified = BCrypt.Net.BCrypt.Verify(model.Password, user.SenhaHash);
        if (!verified)
        {
            if (Request.Method == "GET") return Redirect("/login?error=invalid");
            return Unauthorized(new { message = "Invalid credentials" });
        }

        if (user.Status != "ATIVO")
        {
            if (Request.Method == "GET") return Redirect("/login?error=inactive");
            return Unauthorized(new { message = "User inactive" });
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Nome),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Perfil.ToString()),
            new Claim("perfil", user.Perfil.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // IsPersistent = false: sessao expira ao fechar o browser
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = false,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

        if (Request.Method == "GET") return Redirect("/dashboard");
        return Ok(new { message = "Logged in" });
    }

    // Sem [Authorize]: logout deve funcionar sempre, inclusive com sessao expirada
    [HttpPost("logout")]
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (Request.Method == "GET") return Redirect("/login");
        return Ok(new { message = "Logged out" });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
