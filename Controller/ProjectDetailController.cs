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

    [HttpGet]
    public IActionResult GetAllProjectDetails()
    {
        var projects = _projectDetailRepo.GetAll();
        var projectDetailViews = new List<GetProjectDetailView>();
        foreach (var project in projects)
        {
            var projectDetailView = GetProjectDetailView(project);
            projectDetailViews.Add(projectDetailView);
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

    private GetProjectDetailView GetProjectDetailView(ProjectDetail view)
    {
        GetProjectDetailView detail = new GetProjectDetailView()
        {
            Key = view.Key,
            Description = view.Description,
            Tag = view.Tag,
            Translation = new GetTranslationView
            {
                Key = view.Translation.Key,
                Language = view.Translation.Language,
                Text = view.Translation.Text
            }
        };
        return detail;
    }
    private ProjectDetail ProjectDetailMapper(ProjectDetailView createProjectView)
    {
        ProjectDetail detail = new ProjectDetail()
        {
            Key = createProjectView.Key,
            TranslationId = createProjectView.TranslationId,
            Description = createProjectView.Description,
            Tag = createProjectView.Tag,
        };
        return detail;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProjectDetail([FromBody] ProjectDetailView projectDetailCreate)
    {
    
        var project = _projectDetailRepo.GetAll().Select(x => x.Key).Contains(projectDetailCreate.Key);
        var projectDetail = ProjectDetailMapper(projectDetailCreate);
    
        var projectTranslation = await _translationRepo.GetById(projectDetailCreate.TranslationId);
        if (projectDetail != null)
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