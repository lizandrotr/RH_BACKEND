using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RH_BACKEND.Data;
using RH_BACKEND.dto;
using RH_BACKEND.Infrastructure;
using RH_BACKEND.Services;

namespace RH_BACKEND.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(RhDbContext db, TenantContext tenant, JwtTokenService tokens) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await db.Users.FirstOrDefaultAsync(x =>
            x.TenantId == tenant.CurrentTenantId &&
            x.Username == request.Username &&
            x.IsActive);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Usuario o contraseña inválidos." });

        return Ok(new
        {
            accessToken = tokens.Create(user),
            user = new { user.Id, user.Username, user.FullName, user.Email, user.Role, user.TenantId }
        });
    }
}
