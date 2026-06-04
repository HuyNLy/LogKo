using Logko.API.DTOs.Auth;
using Logko.API.Data;
using Logko.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Logko.API.Services;
public class AuthService: IAuthService
{
    private readonly IUserRepo _userRepo;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepo userRepo, IConfiguration configuration)
    {
        _userRepo = userRepo;
        _configuration = configuration;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            throw new UnauthorizedAccessException("Username and password are required.");
        }

        var user = await _userRepo.GetUserByUsernameAsync(request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        return BuildToken(user);

    }
    public async Task Register(RegisterRequest request)
    {
        if(string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidDataException("Name is required");
        }
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            throw new UnauthorizedAccessException("Username and password are required.");
        }
        if(request.Password != request.ConfirmPassword)
        {
            throw new InvalidOperationException("Password is not matched.");
        }
        await _userRepo.CreateUserAsync( new User
        {
            Name = request.Name,
            Email = request.Email,
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.User
        });
    }

    private LoginResponse BuildToken(User user)
    {
        Claim[] claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name,           user.Username),
            new Claim(ClaimTypes.Role,           user.Role.ToString())
        };

        string jwtKey = _configuration["JwtSettings:Secret"]
            ?? throw new InvalidOperationException("JwtSettings:Secret missing.");

        int expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"]!);

        SymmetricSecurityKey key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

        SigningCredentials creds = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        DateTime expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

        JwtSecurityToken token = new JwtSecurityToken(
            claims:             claims,
            expires:            expires,
            signingCredentials: creds
        );

        string serialized = new JwtSecurityTokenHandler().WriteToken(token);

        return new LoginResponse
        {
            AccessToken  = serialized,
            Role         = user.Role.ToString(),
            ExpiresAt    = expires,
            RefreshToken = "TODO"
        };
    }

}
