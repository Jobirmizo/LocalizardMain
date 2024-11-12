using AutoMapper;
using System.Linq;
using Localizard.DAL.Repositories;
using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Localizard.Controller;

[Route("api/[controller]/[action]")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly IProjectRepo _projectRepo;
    private readonly IProjectDetailRepo _projectDetailRepo;
    private readonly ILanguageRepo _languageRepo;
    private readonly IMapper _mapper;
    
    public ProjectController(IProjectRepo projectRepo, IMapper mapper, IProjectDetailRepo projectDetailRepo, ILanguageRepo languageRepo)
    {
        _projectRepo = projectRepo;
        _mapper = mapper;
        _projectDetailRepo = projectDetailRepo;
        _languageRepo = languageRepo;
    }
    
    
    [HttpGet]
    public  IActionResult GetAllProjects()
    {
        var projects = _projectRepo.GetAllProjects();
        var mappedProjects = _mapper.Map<List<ProjectInfoDto>>(projects);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        return Ok(mappedProjects);
    }
    
   
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        if (!_projectRepo.ProjectExists(id))
            return NotFound();

        var project = await _projectRepo.GetById(id);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(project);
    }



    private ProjectInfo ProjectInfoMapper(ProjectInfoDto projectCreate)
    {
        ProjectInfo projectInfo = new ProjectInfo()
        {
            Name = projectCreate.Name,
            LanguageId = projectCreate.DefaultLanguageId,
        };
        return projectInfo;
    }
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateProject([FromBody] ProjectInfoDto projectCreate)
    {
        if (projectCreate == null)
            return BadRequest(ModelState);

        var project = _projectRepo.GetAllProjects().Select(x => x.Name).Contains(projectCreate.Name);
        var projectInfo =  ProjectInfoMapper(projectCreate);

        var projectDetail = await _projectDetailRepo.GetById(projectCreate.ProjectDetailId);
        if (projectDetail is not null)
        {
            projectInfo.ProjectDetail = projectDetail;
        }

        var languages =  _languageRepo.GetAll();

        foreach (var language in languages)
        {
            if (projectInfo.Languages is null)
                projectInfo.Languages = new List<Language>();
            
            projectInfo.Languages.Add(language);
        }
        
        if (project)
        {
            ModelState.AddModelError("", "Project already exist!");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var projectMap = _mapper.Map<ProjectInfo>(projectCreate);

        if (!_projectRepo.CreateProject(projectMap))
        {
            ModelState.AddModelError("", "Something went wrong! while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }
}