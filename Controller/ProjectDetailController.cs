using AutoMapper;
using Localizard.DAL;
using Localizard.DAL.Repositories;
using Localizard.DAL.Repositories.Implementations;
using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Localizard.Controller;

[Route("api/[controller]/[action]")]
[ApiController]
public class ProjectDetailController : ControllerBase
{
    private readonly IProjectDetailRepo _projectDetail;
    private readonly IMapper _mapper;
    private readonly IProjectDetailRepo _projectDetailRepo;
    private readonly ITranslationRepo _translationRepo;
    public ProjectDetailController(IMapper mapper, IProjectDetailRepo projectDetail, IProjectDetailRepo projectDetailRepo, ITranslationRepo translationRepo)
    {
        _mapper = mapper;
        _projectDetail = projectDetail;
        _projectDetailRepo = projectDetailRepo;
        _translationRepo = translationRepo;
    }

    private GetProjectDetailView GetDetailMapper(ProjectDetail detail)
    {
        GetProjectDetailView detailView = new GetProjectDetailView()
        {
            Key = detail.Key,
            Description = detail.Description,
            Tag = detail.Tag,
            AvailableTranslations = detail.Translation
        };
        return detailView;
    }

    private ProjectDetail CraeteDetailMapper(CreateProjectDetailView create)
    {
        ProjectDetail detailView = new ProjectDetail()
        {
            Key = create.Key,
            Description = create.Description,
            Tag = create.Tag
        };
        return detailView;
    }

    [HttpGet]
    public IActionResult GetAllProjectDetails()
    {
        var projectDetails = _projectDetailRepo.GetAll();

        var projectDetailViews = new List<GetProjectDetailView>();
        foreach (var detial in projectDetails)
        {
            var detailView = GetDetailMapper(detial);
            projectDetailViews.Add(detailView);
        }

        return Ok(projectDetailViews);
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
    public async Task<IActionResult> CreateProjectDetail([FromBody] CreateProjectDetailView detail)
    {
        if (detail == null)
            return BadRequest(ModelState);

        var checkDetail = _projectDetailRepo.GetAll().Select(d => d.Key).Contains(detail.Key);
        var projectDetail = CraeteDetailMapper(detail);
        
        var translations = _translationRepo.GetAll();
        foreach (var translate in translations)
        {
            if (projectDetail.Translation is null)
                projectDetail.Translation = new List<Translation>();
            
            projectDetail.Translation.Add(translate);
        }

        if (checkDetail)
        {
            ModelState.AddModelError("","Project Detail already exists!");
            return StatusCode(500, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_projectDetailRepo.CreateProjectDetail(projectDetail))
        {
            ModelState.AddModelError("","Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created;-)");
    }
    
    [HttpPut]
    public IActionResult UpdateProjectDetail(int projectDetailId, [FromBody] UpdateProjectDetailView updatedProject)
    {
        if (updatedProject == null)
            return BadRequest(ModelState);
    
        if (projectDetailId != updatedProject.Id)
            return BadRequest(ModelState);
    
        if (!_projectDetailRepo.ProjectDetailExist(projectDetailId))
            return NotFound();
    
        if (!ModelState.IsValid)
            return BadRequest();
    
        var mappedProject = _mapper.Map<ProjectDetail>(updatedProject);
    
        if (!_projectDetailRepo.UpdateProjectDetail(mappedProject))
        {
            ModelState.AddModelError("","Something went wrong while updating");
            return StatusCode(500, ModelState);
        }
    
        return NoContent();
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteProjectDetail(int detailId)
    {
        if (!_projectDetailRepo.ProjectDetailExist(detailId))
            return NotFound();

        var deleteDetail = await _projectDetailRepo.GetById(detailId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_projectDetailRepo.DeleteProjectDetail(deleteDetail))
        {
            ModelState.AddModelError("", "Something went wrong while deleting translation");
        }

        return NoContent();
    }

}