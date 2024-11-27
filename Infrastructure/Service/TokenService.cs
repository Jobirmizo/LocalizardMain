using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Localizard.Domain.Entites;
using Microsoft.IdentityModel.Tokens;

namespace Localizard.Service;

public class TokenService
{
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration config)
    {
        var secret = config["Jwt:Key"];
        if (string.IsNullOrEmpty(secret))
        {
            throw new ArgumentNullException(nameof(secret), "JWT secret key is missing or empty. ");
        }

        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }
    
    public string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = "https://localhost:5063",
            Audience = "https://localhost:5063",
            SigningCredentials = credentials
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

}