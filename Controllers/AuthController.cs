using Microsoft.AspNetCore.Mvc;
using Logko.API.Services;
using Logko.API.DTOs.Auth;
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
}