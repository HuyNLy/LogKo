using Microsoft.AspNetCore.Mvc;
using Logko.API.Services;
using Logko.API.DTOs.Auth;
using Logko.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace Logko.API.Controllers;





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
        await _authService.Register(request);
        return StatusCode(201, new { message = "User registered successfully." });    }

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.Login(request);
        return Ok(response);
    }
   

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("Auth controller is working!");
    }

    [Authorize]
    [HttpGet("protected")]
    public IActionResult Protected()
    {
        return Ok("You are authenticated!");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only")]
    public IActionResult AdminOnly()
    {
        return Ok("You are an Admin!");
    }

    [Authorize(Roles = "User")]
    [HttpGet("user-only")]
    public IActionResult UserOnly()
    {
        return Ok("You are a User!");
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var response = await _authService.RefreshToken(request.RefreshToken);
        return Ok(response);
    }
}