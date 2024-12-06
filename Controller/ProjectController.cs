﻿using System.Diagnostics;
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

[Route("api/project/")]
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

   
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllProjects([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        
        var isAdmin = HttpContext.User.IsInRole("Admin");
        
        if (pageNumber <= 0 || pageSize <= 0)
            return BadRequest("PageNumber and PageSize must be greater than zero.");
        
        var query = isAdmin 
            ? _projectRepo.GetAllProjects() 
            : _projectRepo.GetAllProjects().Where(p => p.CreatedBy == userId);
        
        var totalProjects = query.Count();
        
        if (totalProjects == 0)
            return NotFound("No projects found.");
        
        var pagedProjects = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        var projectInfoViews = pagedProjects
            .Select(project => ProjectViewMapper(project))
            .ToList();
        return Ok(new
        {
            TotalCount = totalProjects,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalProjects / (double)pageSize),
            Projects = projectInfoViews
        });
    }

    
    [HttpGet("get-by/{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var project = await _projectRepo.GetById(id);

        if (project == null)
            return NotFound(ModelState);

        var projectView = ProjectViewMapper(project);

        return Ok(projectView);
    }
    
    
    
    [HttpPost("create")]
    public IActionResult CreateProject([FromBody] CreateProjectView create)
    {
        if (create == null)
            return BadRequest(ModelState);

        var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var project =  _projectRepo.GetAllProjects().Select(x => x.Name).Contains(create.Name);
        
        var projectInfo =  ProjectInfoMapper(create);
        projectInfo.CreatedBy = userId;

        var languages =   _languageRepo.GetAll();

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
            return BadRequest();

        if (!_projectRepo.CreateProject(projectInfo))
        {
            ModelState.AddModelError("", "Something went wrong! while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }

    [HttpPut("update/{id}")]
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
            return BadRequest();

        
        if (!_projectRepo.UpdateProject(existingProject))
        {
            ModelState.AddModelError("", "Something went wrong while saving the project.");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully updated the project.");
    }

    [HttpDelete("delete/{id}")]
    public IActionResult DeleteProject(int id)
    {
        if (!_projectRepo.DeleteProject(id))
        {
            return NotFound(new { message = "Project not found or could not be deleted" });
        }
        
        return Ok(new { message = "Project deleted successfully" });
    }

    #region CreateProjectMapper
    private ProjectInfo ProjectInfoMapper(CreateProjectView createProjectCreate)
    {
        ProjectInfo projectInfo = new ProjectInfo()
        {
            Name = createProjectCreate.Name,
            LanguageId = createProjectCreate.DefaultLanguageId
        };
        return projectInfo;
    }
    #endregion
    #region GetProjectsMannualMapper
    private GetProjectView ProjectViewMapper(ProjectInfo projectInfo)
    {
        GetProjectView createProjectView = new GetProjectView()
        {
            Id = projectInfo.Id,
            Name = projectInfo.Name,
            CreatedAt = projectInfo.CreatedAt,
            UpdatedAt = projectInfo.UpdatedAt,
            DefaultLanguageId = projectInfo.LanguageId,
            AvailableLanguageIds = projectInfo.Languages.Select(lang => lang.Id).ToArray()
        };
        return createProjectView;
    }
    #endregion
    


}