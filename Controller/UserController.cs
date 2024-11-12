using AutoMapper;
using Localizard.DAL;
using Localizard.DAL.Repositories;
using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Localizard.Controller;

[Route("api/[controller]/[action]")]
[ApiController]
public class UserController : ControllerBase
{
    
    private readonly IUserManager _userManager;
    private readonly IMapper _mapper;
    
    public UserController(IUserManager userManager, IMapper mapper )
    {
        _userManager = userManager;
        _mapper = mapper;
    }
    
    
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _userManager.GetAllUsers();
        var mappedUsers = _mapper.Map<List<GetUsersDto>>(users);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(mappedUsers);
    }
    
    [AllowAnonymous]
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
    
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreateUser([FromBody] UserDto userCreate)
    {
        if (userCreate == null)
            return BadRequest(ModelState);

        var user = _userManager.GetAllUsers()
            .Where(p => p.Username.Trim().ToUpper() == userCreate.Username.TrimEnd().ToUpper())
            .FirstOrDefault();

        if (user != null)
        {
            ModelState.AddModelError("", "Project already exist!");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userMap = _mapper.Map<User>(userCreate);

        if (!_userManager.CreateUser(userMap))
        {
            ModelState.AddModelError("", "Something went wrong! while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }
}