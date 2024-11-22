using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Localizard.DAL;
using Localizard.Domain.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Localizard.Services;

public class JwtService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    public JwtService(IConfiguration config, AppDbContext context)
    {
        _config = config;
        _context = context;
    }

   public async Task<LoginResponseView?> Authenticate(LoginView request) 
   {
    
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return null;
        
        var userAccount = await _context.Users.FirstOrDefaultAsync(x => x.Username == request.Username);

        
        if (userAccount is null || !BCrypt.Net.BCrypt.Verify(request.Password, userAccount.Password))
            return null;

       
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];
        var key = _config["Jwt:Key"];
        var tokenValidityMins = _config.GetValue<int>("Jwt:TokenValidityMins");

        
        var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);


        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Name, request.Username)
            }),
            Expires = tokenExpiryTimeStamp,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), 
                SecurityAlgorithms.HmacSha512Signature) 
        };

        // Create and write the token
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);

        
        return new LoginResponseView()
        {
            AccessToken = accessToken,  
            Username = request.Username, 
            ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds 
        }; 
   }

}