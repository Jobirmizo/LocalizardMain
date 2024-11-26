using AutoMapper;
using System.Linq;
using System.Security.Claims;
using Localizard.DAL;
using Localizard.DAL.Repositories;
using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Localizard.Controller;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IProjectRepo _projectRepo;
    private readonly IProjectDetailRepo _projectDetailRepo;
    private readonly ILanguageRepo _languageRepo;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;
    
    public ProjectController(IProjectRepo projectRepo, IMapper mapper, IProjectDetailRepo projectDetailRepo, ILanguageRepo languageRepo, AppDbContext context)
    {
        _projectRepo = projectRepo;
        _mapper = mapper;
        _projectDetailRepo = projectDetailRepo;
        _languageRepo = languageRepo;
        _context = context;
    }


    [HttpGet]
    public  async Task<IActionResult> GetAllProjects()
    {
        var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        
        var projects = _projectRepo.GetAllProjects()
            .Where(p => p.CreatedBy == userId)
            .ToList();
        
        if (!projects.Any())
            return NotFound("No projects found for the current user.");
        
        
        var projectInfoViews = new List<GetProjectView>(); 
        foreach (var project in projects)
        {
            var projectinfoView = ProjectViewMapper(project);
            projectinfoView.DefaultLanguage = await _languageRepo.GetById(project.LanguageId);
            projectInfoViews.Add(projectinfoView);
        }
        return Ok(projectInfoViews);
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


    [HttpGet]
    public async Task<IActionResult> ProjectsPagination(int page = 1, int pageSize = 10)
    {
        var totalCount = _projectRepo.GetAllProjects().Count();
        var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

        var projectsPage = _projectRepo
            .GetAllProjects()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var response = new
        {
            page,
            pageSize,
            totalCount,
            totalPages,
            data = projectsPage
        };

        return Ok(response);
    }
    
    
    
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectView create)
    {
        if (create == null)
            return BadRequest(ModelState);

        var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var project = _projectRepo.GetAllProjects().Select(x => x.Name).Contains(create.Name);
        
        var projectInfo =  ProjectInfoMapper(create);
        projectInfo.CreatedBy = userId;

        // var projectDetials = _projectDetailRepo.GetAll();
        //
        // foreach (var detial in projectDetials)
        // {
        //     if (projectInfo.ProjectDetail is null)
        //         projectInfo.ProjectDetail = new List<ProjectDetail>();
        //     
        //     if(create.ProjectDetailIds.Contains(detial.Id))
        //         projectInfo.ProjectDetail.Add(detial);
        // }

        var languages =  _languageRepo.GetAll();

        foreach (var language in languages)
        {
            if(projectInfo.Languages is null)
                projectInfo.Languages = new List<Language>();
            if(create.AvailableLanguageIds.Contains(language.Id))
                projectInfo.Languages.Add(language);
        }
        
        if (project)
        {
            ModelState.AddModelError("", "Project already exist!");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_projectRepo.CreateProject(projectInfo))
        {
            ModelState.AddModelError("", "Something went wrong! while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }

    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectView update)
    {
        if (update == null)
            return BadRequest(ModelState);
        
        var existingProject = await _projectRepo.GetById(id);
        
        if (existingProject == null)
            return NotFound($"Project with ID {id} not found.");
        
        
        var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
    
        var projectExists = _projectRepo.GetAllProjects().Any(x => x.Name == update.Name && x.Id != id);
        
        if (projectExists)
        {
            ModelState.AddModelError("", "Project with this name already exists.");
            return StatusCode(422, ModelState);
        }
        
        existingProject.Name = update.Name; 
        existingProject.LanguageId = update.DefaultLanguageId;
        existingProject.UpdatedAt = DateTime.UtcNow;
        
        
        var languages = _languageRepo.GetAll();
        existingProject.Languages.Clear(); 
        
        foreach (var language in languages)
        {
            if (update.AvailableLanguageIds.Contains(language.Id))
                existingProject.Languages.Add(language);
        }
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        
        if (!_projectRepo.UpdateProject(existingProject))
        {
            ModelState.AddModelError("", "Something went wrong while saving the project.");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully updated the project.");
    }


    #region CreateProjectMapper
    private ProjectInfo ProjectInfoMapper(CreateProjectView createProjectCreate)
    {
        ProjectInfo projectInfo = new ProjectInfo()
        {
            Name = createProjectCreate.Name,
            LanguageId = createProjectCreate.DefaultLanguageId,
            CreatedAt = createProjectCreate.CreatedAt,
            UpdatedAt = createProjectCreate.UpdatedAt
        };
        return projectInfo;
    }
    #endregion
    #region GetProjectsMannualMapper
    private GetProjectView ProjectViewMapper(ProjectInfo projectInfo)
    {
        GetProjectView createProjectView = new GetProjectView()
        {
            Name = projectInfo.Name,
            CreatedAt = projectInfo.CreatedAt,
            UpdatedAt = projectInfo.UpdatedAt,
            AvialableLanguages = projectInfo.Languages
        };
        return createProjectView;
    }
    #endregion
    


}