using AutoMapper;
using Localizard.DAL;
using Localizard.DAL.Repositories;
using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Localizard.Controller;

[Route("api/[controller]/[action]")]
[ApiController]
public class UserController : ControllerBase
{
    
    private readonly IUserManager _userManager;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;
    
    public UserController(IUserManager userManager, IMapper mapper, AppDbContext context)
    {
        _userManager = userManager;
        _mapper = mapper;
        _context = context;
    }
    
    
    [HttpGet]
    public IActionResult GetAllUsers(int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        var users = _userManager.GetAllUsers();
        var totalCount = users.Count();
        var usersPage = users
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        var mappedUsers = _mapper.Map<List<GetUsersView>>(usersPage);
        var response = new
        {
            currentPage = page,
            pageSize,
            totalCount,
            totalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            data = mappedUsers,
        };
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(response);
    }
    
        
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        if (!_userManager.UserExists(id))
            return NotFound();

        var user = await _userManager.GetByIdAsync(id);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(user);
    }
    

    [HttpPut]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] User update)
    {
        if (update == null || !ModelState.IsValid)
        {
            return BadRequest("Invalid Request");
        }

        var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == update.Id);
        if (user == null)
            return NotFound(new { message = "User not found" });

        user.Username = update.Username ?? user.Username;
        user.Role = update.Role ?? user.Role;

        if (!string.IsNullOrEmpty(update.Password))
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(update.Password);
            user.Password = hashedPassword;
        }
        
          
        if (!_userManager.UpdateUser(user))
        {
            ModelState.AddModelError("", "Something went wrong while saving the user.");
            return StatusCode(500, ModelState);
        }

        return Ok(new { message = "Updated" });
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        if (!_userManager.UserExists(userId))
            return NotFound();

        var userDelete = await _userManager.GetByIdAsync(userId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_userManager.DeleteUser(userDelete))
        {
            ModelState.AddModelError("", "Something went wrong while deleting translation");
        }

        return NoContent();
    }
}