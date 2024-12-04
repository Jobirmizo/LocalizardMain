using System.Collections;
using AutoMapper;
using Localizard.DAL;
using Localizard.DAL.Repositories;
using Localizard.DAL.Repositories.Implementations;
using Localizard.Data.Entites;
using Localizard.Domain.Entites;
using Localizard.Domain.Enums;
using Localizard.Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Localizard.Controller;

[Route("api/project-detail")]
[ApiController]
[Authorize]
public class ProjectDetailController : ControllerBase
{
    private readonly IProjectDetailRepo _projectDetail;
    private readonly IProjectDetailRepo _projectDetailRepo;
    private readonly IProjectRepo _project;
    private readonly ITranslationRepo _translationRepo;
    private readonly ITagRepo _tag;
    public ProjectDetailController(IMapper mapper, IProjectDetailRepo projectDetail, 
        IProjectDetailRepo projectDetailRepo, ITranslationRepo translationRepo, ITagRepo tag, AppDbContext context, IProjectRepo project)
    {
        _projectDetail = projectDetail;
        _projectDetailRepo = projectDetailRepo;
        _translationRepo = translationRepo;
        _tag = tag;
        _project = project;
    }
    
    
    [HttpGet("get-all")]
    public IActionResult GetAllProjectDetails( int projectId, string? Search = null)
    {
        var projectDetails = _projectDetailRepo.GetAll();
        
        var filterDetail = projectDetails
            .Where(d => d.ProjectInfoId == projectId)
            .ToList();
        var data = Array.Empty<object>();
        
        if (!filterDetail.Any())
        {
            return NotFound(data);
        }
        

        if (!string.IsNullOrEmpty(Search))
        {
            var detailView = projectDetails.Select(detail => GetDetailMapper(detail)).ToList();
            
            detailView = detailView
                .Where(pd =>
                    (pd.Key != null && pd.Key.IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0) || 
                    (pd.Tags != null && pd.Tags.Any(
                        tag => tag.Name.IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0))) // Use tag.Name instead
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

        var checkDetail =  _projectDetailRepo.GetAll().Select(d => d.Key).Contains(detail.Key);
        var tags = _tag.GetAllAsync();
        var validTagIds = tags.Select(t => t.Id).ToList();    
        var invalidTags = detail.TagIds.Except(validTagIds).ToList();
        
        if (invalidTags.Any())
            return BadRequest($"Invalid tag IDs: {string.Join(", ", invalidTags)}");
        
        var projectDetail = CraeteDetailMapper(detail);

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
    
    [HttpPut("update")]
    public async Task<IActionResult> UpdateProjectDetail(int id, [FromBody] UpdateProjectDetailView update)
    {
        if (update == null)
            return BadRequest(ModelState);

        var existingDetail = await _projectDetailRepo.GetById(id);

        if (existingDetail == null)
            return NotFound("there is no such detial");

        var projectDetialExists = _projectDetailRepo.GetAll().Any(x => x.Key == update.Key && x.Id != id);

        if (projectDetialExists)
        {
            ModelState.AddModelError("","Project Detail already exist, check it again!");
            return StatusCode(422, ModelState);
        }

        existingDetail.Key = update.Key;
        existingDetail.Description = update.Description;

        var translations = _translationRepo.GetAll();
        existingDetail.Translation.Clear();

        foreach (var translate in translations)
        {
            if (update.TranslationIds.Contains(translate.Id)) 
                existingDetail.Translation.Add(translate);
        }

        var tags = _tag.GetAllAsync();
        existingDetail.Tags.Clear();

        foreach (var tag in tags)
        {
            if (update.TagIds.Contains(tag.Id)) 
                existingDetail.Tags.Add(tag);
        }
            
        if (!ModelState.IsValid)
            return BadRequest();

        
        if (!_projectDetailRepo.UpdateProjectDetail(existingDetail))
        {
            ModelState.AddModelError("", "Something went wrong while saving the project Detail.");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully updated the project.");
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
            AvailableTranslations = detail.Translation,
            Tags = detail.Tags?.Select(tag => new Tag
            {
                Id = tag.Id,
                Name = tag.Name
            }).ToList() ?? new List<Tag>(),
            TagIds = detail.Tags.Select(tag => tag.Id).ToArray()
        };
        
        return detailView;
    }
    #endregion
    #region CreateDetailMapper
    private ProjectDetail CraeteDetailMapper(CreateProjectDetailView create)
    {
        ProjectDetail detailView = new ProjectDetail()
        {
            Key = create.Key,
            ProjectInfoId = create.ProjectInfoId,
            Description = create.Description,
            Translation = new List<Translation>(),
            Tags = new List<Tag>()
        };
        if (create.Translations != null)
        {
            foreach (var translate in create.Translations)
            {
                var translation = new Translation()
                {
                    SymbolKey = translate.SymbolKey,
                    LanguageId = translate.LanguageId,
                    Text = translate.Text
                };
                
                detailView.Translation.Add(translation);
            }
        }

        return detailView;
    }
    #endregion

}