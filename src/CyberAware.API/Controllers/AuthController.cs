using System.Security.Claims;
using CyberAware.Business.DTOs;
using CyberAware.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CyberAware.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var (success, message) = await _authService.RegisterAsync(request);

        if (!success)
            return BadRequest(new { message });

        return Ok(new { message });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }

    [HttpPost("verify-2fa")]
    public async Task<IActionResult> VerifyTwoFactor([FromBody] Verify2FARequest request)
    {
        var response = await _authService.VerifyTwoFactorAsync(request);

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }

    [Authorize]
    [HttpPost("setup-2fa")]
    public async Task<IActionResult> SetupTwoFactor()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(new { message = "Invalid token." });

        var userId = int.Parse(userIdClaim);
        var response = await _authService.SetupTwoFactorAsync(userId);

        if (response == null)
            return NotFound(new { message = "User not found." });

        return Ok(response);
    }
}