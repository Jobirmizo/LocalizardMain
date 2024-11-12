using AutoMapper;
using Localizard.DAL;
using Localizard.DAL.Repositories;
using Localizard.DAL.Repositories.Implementations;
using Localizard.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Localizard.Controller;

[Route("api/[controller]/[action]")]
[ApiController]
public class ProjectDetailController : ControllerBase
{
    private readonly IProjectDetailRepo _projectDetail;
    private readonly IMapper _mapper;
    public ProjectDetailController(AppDbContext context, IMapper mapper, IProjectDetailRepo projectDetail)
    {
        _mapper = mapper;
        _projectDetail = projectDetail;
    }

    [HttpGet]
    public IActionResult GetAllProjectDetails()
    {
        var projectDetail = _projectDetail.GetAll();
        var mappedProjectDetails = _mapper.Map<List<ProjectDetailDto>>(projectDetail);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        return Ok(mappedProjectDetails);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectDetailById(int id)
    {
        if (!_projectDetail.ProjectDetailExist(id))
            return NotFound();

        var projectDetail = await _projectDetail.GetById(id);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(projectDetail);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProjectDetail([FromBody] ProjectDetailDto projectDetailCreate)
    {
        if (projectDetailCreate == null)
            return BadRequest(ModelState);

        var projectDetail = _projectDetail.GetAll()
            .Where(p => p.Key.Trim().ToUpper() == projectDetailCreate.Key.TrimEnd().ToUpper())
            .FirstOrDefault();

        if (projectDetail != null)
        {
            ModelState.AddModelError("","Project already exist");
            return StatusCode(422, ModelState);
        }

        return Ok("Successfully created");
    }
    
}