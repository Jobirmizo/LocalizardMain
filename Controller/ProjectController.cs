using AutoMapper;
using System.Linq;
using Localizard.DAL.Repositories;
using Localizard.DAL.Repositories.Implementations;
using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    
    public ProjectController(IProjectRepo projectRepo, IMapper mapper, IProjectDetailRepo projectDetailRepo, ILanguageRepo languageRepo)
    {
        _projectRepo = projectRepo;
        _mapper = mapper;
        _projectDetailRepo = projectDetailRepo;
        _languageRepo = languageRepo;
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

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult>UsersPagination(int page = 1, int pageSize = 10)
    {
        var totalCount  = _projectRepo.GetAllProjects().Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
        var projectsPage =  _projectRepo.GetAllProjects().Skip((page - 1) * pageSize).Take(pageSize).ToList();

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

    private GetProjectView ProjectViewMapper(ProjectInfo projectInfo)
    {
        GetProjectView createProjectView = new GetProjectView()
        {
            Name = projectInfo.Name,
            CreatedAt = projectInfo.CreatedAt,
            UpdatedAt = projectInfo.UpdatedAt,
            ProjectDetail = projectInfo.ProjectDetail,
            AvialableLanguages = projectInfo.Languages
        };
        return createProjectView;
    }
    
    
    
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
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
        
        var projectDetail = await _projectDetailRepo.GetById(create.ProjectDetailId);
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

        if (!_projectRepo.CreateProject(projectInfo))
        {
            ModelState.AddModelError("", "Something went wrong! while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }

    [HttpPut("{projectId}")]
    public IActionResult UpdateProject(int projectId, [FromBody] UpdateProjectView updatedProject)
    {
        if (updatedProject == null)
            return BadRequest(ModelState);
    
        if (projectId != updatedProject.Id)
            return BadRequest(ModelState);
    
        if (!_projectRepo.ProjectExists(projectId))
            return NotFound();
    
        if (!ModelState.IsValid)
            return BadRequest();
    
        var projectMap = _mapper.Map<ProjectInfo>(updatedProject);
    
        if (!_projectRepo.UpdateProject(projectMap))
        {
            ModelState.AddModelError("","Something went wrong while updating");
            return StatusCode(500, ModelState);
        }
    
        return NoContent();
    }
}