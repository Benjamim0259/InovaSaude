using InovaSaude.Blazor.Models;
using InovaSaude.Blazor.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            return BadRequest(new { message = "Email and password required" });

        var user = await _usuarioService.GetUsuarioByEmailAsync(model.Email);
        if (user == null) return Unauthorized(new { message = "Invalid credentials" });

        var verified = BCrypt.Net.BCrypt.Verify(model.Password, user.SenhaHash);
        if (!verified) return Unauthorized(new { message = "Invalid credentials" });

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Nome),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("perfil", user.Perfil.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Ok(new { message = "Logged in" });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Logged out" });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
