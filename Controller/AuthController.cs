using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Localizard.DAL;
using Localizard.DAL.Repositories;
using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;
using Localizard.Service;
using Localizard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Localizard.Controller;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class AuthController : ControllerBase
{
    
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly JwtService _jwtService;
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;
    
    public AuthController(IMapper mapper, IConfiguration configuration, JwtService jwtService, AppDbContext context, IUserManager userManager, TokenService tokenService) 
    {
        _mapper = mapper;
        _configuration = configuration;
        _jwtService = jwtService;
        _context = context;
        _tokenService = tokenService;
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<LoginResponseView>> Login(LoginView request)
    {
        if (request == null)
        {
            return BadRequest("Invalid client request");
        }

        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }
        
        var token = _tokenService.GenerateJwtToken(user);
        var response = new AuthResponseView.AuthResponse(token, user.Username, user.Role);
        
        return Ok(new
        {token, user.Role});
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterView model)
    {
        if (model == null || !ModelState.IsValid)
        {
            return BadRequest("Invalid registration request");
              
        }
          

        var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Username == model.Username);
        if (existingUser != null)
        {
            return Conflict(new { message = "Username already exists" });
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

        var user = new User
        {
            Username = model.Username,
            Password = hashedPassword,
            Role = model.Role 
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var token =_tokenService.GenerateJwtToken(user);
        
        return Ok(new
        {
            token,
            message = "User registered successfully"

        });
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> UsersPagination(int page = 1, int pageSize = 10)
    {
        var totalRecords = await _context.Users.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        var users = await _context.Users.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var response = new
        {
            page,
            pageSize,
            totalRecords,
            totalPages,
            data = users
        };
        return Ok(response);
    }
    
    
    
}