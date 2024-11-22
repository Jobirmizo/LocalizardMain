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
        var projects = _projectRepo.GetAllProjects();
        
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
    public IEnumerable<ProjectInfo> Pagination(int page = 1, int pageSize = 10)
    {
        var totalCount = _projectRepo.GetAllProjects().Count();
        var totalPages = (int)Math.Ceiling((decimal)totalCount / 10);
        var projectsPage = _projectRepo.GetAllProjects().Skip((totalPages - 1) * 10).Take(10).ToList();
        return projectsPage;
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
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectView createProjectCreate)
    {
        if (createProjectCreate == null)
            return BadRequest(ModelState);

        var project = _projectRepo.GetAllProjects().Select(x => x.Name).Contains(createProjectCreate.Name);
        var projectInfo =  ProjectInfoMapper(createProjectCreate);

        var projectDetail = await _projectDetailRepo.GetById(createProjectCreate.ProjectDetailId);
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

        var projectMap = _mapper.Map<ProjectInfo>(createProjectCreate);

        if (!_projectRepo.CreateProject(projectMap))
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