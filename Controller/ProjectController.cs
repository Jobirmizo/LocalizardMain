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

        var projectDetials = _projectDetailRepo.GetAll();

        foreach (var detial in projectDetials)
        {
            if (projectInfo.ProjectDetail is null)
                projectInfo.ProjectDetail = new List<ProjectDetail>();
            
            if(create.ProjectDetailIds.Contains(detial.Id))
                projectInfo.ProjectDetail.Add(detial);
        }

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
        var currentUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (update == null || id != update.Id)
            return BadRequest("Invalid request.");
        
        if (string.IsNullOrEmpty(currentUser))
            return Unauthorized("User not authenticated.");


        var existingProject = await _context.Projects.Include(p => p.LanguageId)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existingProject == null)
            return NotFound("Project not found.");

        
        if (!string.Equals(existingProject.CreatedBy, currentUser, StringComparison.OrdinalIgnoreCase))
            return Forbid();

        
        existingProject.Name = update.Name;
        existingProject.LanguageId = update.DefaultLanguageId;
        existingProject.ProjectDetailId = update.ProjectDetailId;
        existingProject.UpdatedAt = DateTime.UtcNow;
        
        var existingLanguages = existingProject.Languages.ToList();
        
        var newLanguages = _context.Languages
            .Where(lang => update.AvailableLanguageIds.Contains(lang.Id) &&
                           !existingLanguages.Any(el => el.Id == lang.Id))
            .ToList();
        
        var languagesToRemove = existingLanguages
            .Where(el => !update.AvailableLanguageIds.Contains(el.Id))
            .ToList();
        
        foreach (var lang in languagesToRemove)
        {
            existingProject.Languages.Remove(lang);
        }

     
        foreach (var lang in newLanguages)
        {
            existingProject.Languages.Add(lang);
        }

        
        await _context.SaveChangesAsync();

        return Ok(existingProject);
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
            AvialableProjectDetails = projectInfo.ProjectDetail,
            AvialableLanguages = projectInfo.Languages
        };
        return createProjectView;
    }
    #endregion

   
}