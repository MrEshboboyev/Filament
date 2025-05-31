using Filament.WebApi.Models.DTOs;
using Filament.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Filament.WebApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await authService.RegisterAsync(model);
        if (result == null)
            return BadRequest("User registration failed");

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await authService.LoginAsync(model);
        if (result == null)
            return Unauthorized("Invalid credentials");

        return Ok(result);
    }
}
