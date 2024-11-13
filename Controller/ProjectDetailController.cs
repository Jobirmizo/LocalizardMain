using AutoMapper;
using Localizard.DAL;
using Localizard.DAL.Repositories;
using Localizard.DAL.Repositories.Implementations;
using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Localizard.Controller;

[Route("api/[controller]/[action]")]
[ApiController]
public class ProjectDetailController : ControllerBase
{
    private readonly IProjectDetailRepo _projectDetail;
    private readonly IProjectRepo _projectRepo;
    private readonly IMapper _mapper;
    private readonly IProjectDetailRepo _projectDetailRepo;
    private readonly ITranslationRepo _translationRepo;
    public ProjectDetailController(IMapper mapper, IProjectDetailRepo projectDetail, IProjectRepo projectRepo, IProjectDetailRepo projectDetailRepo, ITranslationRepo translationRepo)
    {
        _mapper = mapper;
        _projectDetail = projectDetail;
        _projectRepo = projectRepo;
        _projectDetailRepo = projectDetailRepo;
        _translationRepo = translationRepo;
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

    private ProjectDetail ProjectDetailMapper(ProjectDetailDto mapperTranslation)
    {
        ProjectDetail translation = new ProjectDetail()
        {
            Key = mapperTranslation.Key,
            TranslationId = mapperTranslation.TranslationId,
            Description = mapperTranslation.Description,
            Tag = mapperTranslation.Tag
        };
        
        return translation;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProjectDetail([FromBody] ProjectDetailDto projectDetailCreate)
    {
        if (projectDetailCreate == null)
            return BadRequest(ModelState);
    
        var project = _projectDetailRepo.GetAll().Select(x => x.Key).Contains(projectDetailCreate.Key);
        var projectDetail = ProjectDetailMapper(projectDetailCreate);
    
        var projectTranslation = await _translationRepo.GetById(projectDetailCreate.TranslationId);
        if (projectTranslation is not null)
        {
            projectDetail.Translation = projectTranslation;
        }

        if (project)
        {
            ModelState.AddModelError("", "Project Key is Already exist");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var mappedProjectDetail = _mapper.Map<ProjectDetail>(projectDetailCreate);

        if (!_projectDetailRepo.CreateProjectDetail(mappedProjectDetail))
        {
            ModelState.AddModelError("", "Something went wrong");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }
    
}