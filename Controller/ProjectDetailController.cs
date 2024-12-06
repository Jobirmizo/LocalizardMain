using System.Collections;
using System.Diagnostics;
using AutoMapper;
using Localizard.DAL;
using Localizard.DAL.Repositories;
using Localizard.DAL.Repositories.Implementations;
using Localizard.Data.Entites;
using Localizard.Domain.Entites;
using Localizard.Domain.Enums;
using Localizard.Domain.ViewModel;
using Localizard.Features.Translation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Localizard.Controller;

[Route("api/project-detail")]
[ApiController]
[Authorize]
public class ProjectDetailController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IProjectDetailRepo _projectDetail;
    private readonly IProjectDetailRepo _projectDetailRepo;
    private readonly IProjectRepo _projectRepo;
    private readonly ITranslationRepo _translationRepo;
    private readonly ITagRepo _tag;
    private readonly IMapper _mapper;
    
    public ProjectDetailController(IMapper mapper, IProjectDetailRepo projectDetail, 
        IProjectDetailRepo projectDetailRepo, ITranslationRepo translationRepo, ITagRepo tag, AppDbContext context, IProjectRepo project, IProjectRepo projectRepo)
    {
        _mapper = mapper;
        _projectDetail = projectDetail;
        _projectDetailRepo = projectDetailRepo;
        _translationRepo = translationRepo;
        _tag = tag;
        _context = context;
        _projectRepo = projectRepo;
    }
    
    
    [HttpGet("get-all")]
    public IActionResult GetAllProjectDetails( int projectId, string? Search = null)
    {
        
        var filterDetail = _context.ProjectDetails.Include(x => x.Translations).Include(t => t.Tags)
            .Where(d => d.ProjectInfoId == projectId).OrderBy(p => p.Id).ToList();
        
        var data = Array.Empty<object>();
        
        if (!filterDetail.Any())
        {
            return NotFound(data);
        }
    
      
        if (!string.IsNullOrEmpty(Search))
        {
            var detailView = filterDetail.Select(detail => GetDetailMapper(detail)).ToList();
            
            detailView = detailView
                .Where(pd =>
                    (pd.Key != null && pd.Key.IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0) || 
                    (pd.Tags != null && pd.Tags.Any(
                        tag => tag.Name.IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0)))
                .ToList();

            return Ok(detailView);
        }
        
        var allProjectDetailViews = filterDetail.Select(detail => GetDetailMapper(detail)).ToList();
        
        return Ok(allProjectDetailViews);
    }
    
    [HttpGet("get-by/{id}")]
    public async Task<IActionResult> GetProjectDetailById(int id)
    {
        if (!_projectDetail.ProjectDetailExist(id))
            return NotFound();

        var projectDetail = await _projectDetail.GetById(id);

        if (!ModelState.IsValid)
            return BadRequest();  

        return Ok(projectDetail);
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateProjectDetail([FromBody] CreateProjectDetailView detail)
    {
        
        if (detail == null)
            return BadRequest(ModelState);
       
        
        var watch2 = Stopwatch.StartNew();
        var checkDetail =  _context.ProjectDetails.Any(d => d.Key == detail.Key);
        
        
        var tags=_tag.GetAllAsync();
        
        
        var validTagIds = tags.Select(t => t.Id).ToList();    
        var invalidTags = detail.TagIds.Except(validTagIds).ToList();
      
        
        
        if (invalidTags.Any())
            return BadRequest($"Invalid tag IDs: {string.Join(", ", invalidTags)}");
        
       
        var project = await _projectRepo.GetById(detail.ProjectInfoId);

        if (project is null) return BadRequest();
       
        
        
        var LaguageIds = project.Languages.Select(x => x.Id);
        var projectDetail = CraeteDetailMapper(detail, LaguageIds);

        foreach (var tag in tags)
        {
            if (projectDetail.Tags is null)
                projectDetail.Tags = new List<Tag>();
            if(detail.TagIds.Contains(tag.Id))
                projectDetail.Tags.Add(tag);
        }
        
        
        
        if (checkDetail)
        {
            ModelState.AddModelError("","Project Detail already exists!");
            return StatusCode(500, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest();

        if (!_projectDetailRepo.CreateProjectDetail(projectDetail))
        {
            ModelState.AddModelError("","Something went wrong while saving");
            return StatusCode(500, ModelState);
        }
        

        return Ok("Successfully created;-)");
    }
    
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateProjectDetail(int id, [FromBody] UpdateProjectDetailView detail)
    {

        if (detail == null)
            return BadRequest();
        
        var projectDetail = await _projectDetailRepo.GetById(id);
        

        if (projectDetail == null)
            return NotFound("Project Detial not found");
        

        projectDetail.Key = detail.Key;
        projectDetail.Description = detail.Description;
        
        
        var tags = _tag.GetAllAsync();
        
        if(projectDetail is  null)
            projectDetail.Tags = new List<Tag>();
        
        projectDetail.Tags.Clear();
        
        foreach (var tag in tags)
        {
            if(detail.TagIds.Contains(tag.Id))
                projectDetail.Tags.Add(tag);
        }
        
        var project = await _projectRepo.GetById(projectDetail.ProjectInfoId);
        
        var LaguageIds = project.Languages.Select(x => x.Id);
        
        projectDetail.Translations.Clear();
        foreach (var item in detail.Translations)
        {
            
            if (LaguageIds.Contains(item.LanguageId))
            {
                Translation translation = new Translation();
                translation.LanguageId = item.LanguageId;
                translation.SymbolKey = item.SymbolKey;
                translation.Text = item.Text;
                    
                projectDetail.Translations.Add(translation);
            }
           
        }

        if (!ModelState.IsValid)
            return BadRequest();

        if (!_projectDetailRepo.UpdateProjectDetail(projectDetail))
        {
            ModelState.AddModelError("","Something went wrong while saving the project Detial");
            return StatusCode(500, ModelState);
        }
        

        return Ok("Successfully updated");
    }

    
    [HttpDelete("delete")]
    public IActionResult DeleteProjectDetail(int id)
    {
        if (!_projectDetailRepo.DeleteProjectDetail(id))
        {
            return NotFound(new { message = "Project Detail not Found!" });
        }

        return Ok(new { message = "Project Detail removed!" });
    }
    
    #region GetDetailMapper
    private GetProjectDetailView GetDetailMapper(ProjectDetail detail)
    {
        GetProjectDetailView detailView = new GetProjectDetailView()
        {
            Id = detail.Id,
            ProjectInfoId = detail.ProjectInfoId,
            Key = detail.Key,
            Description = detail.Description,
            AvailableTranslations = detail.Translations,
            Tags = detail.Tags?.Select(tag => new Tag
            {
                Id = tag.Id,
                Name = tag.Name
            }).ToList() ?? new List<Tag>(),
        };
        
        return detailView;
    }
    #endregion
    #region CreateDetailMapper
    private ProjectDetail CraeteDetailMapper(CreateProjectDetailView create,IEnumerable<int>  Languages)
    {
        ProjectDetail detailView = new ProjectDetail()
        {
            Key = create.Key,
            ProjectInfoId = create.ProjectInfoId,
            Description = create.Description,
            Translations = new List<Translation>(),
            TagIds = create.TagIds
        };
        if (create.Translations != null)
        {
            foreach (var translate in create.Translations)
            {
                if (Languages.Contains(translate.LanguageId))
                {
                    var translation = new Translation()
                    {
                        SymbolKey = translate.SymbolKey,
                        LanguageId = translate.LanguageId,
                        Text = translate.Text
                    };
                    
                    detailView.Translations.Add(translation);
                }
                
            }
        }

        return detailView;
    }
    #endregion
    
}