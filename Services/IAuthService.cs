using Logko.API.DTOs.Auth;

namespace Logko.API.Services;
public interface IAuthService
{
    Task<LoginResponse> Login(LoginRequest request);
    Task Register(RegisterRequest request);
}