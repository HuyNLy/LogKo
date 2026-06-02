using Microsoft.AspNetCore.Mvc;
namespace Logko.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("Auth controller is working!");
    }
}