using Localizard.DAL.Repositories;
using Localizard.Data.Entites;
using Localizard.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Localizard.Controller;
[Route("api/tag")]
[ApiController]
[Authorize]
public class TagController : ControllerBase
{

    private readonly ITagRepo _tag;

    public TagController(ITagRepo tag)
    {
        _tag = tag;
    }

    [HttpGet("get-all")]
    public IActionResult GetAllTags()
    {
        var tags = _tag.GetAllAsync();
        return Ok(tags);
    }

    [HttpGet("get-by/{id}")]
    public async Task<IActionResult> CreateTag(int id)
    {
        var tag = await _tag.GetById(id);
        return Ok(tag);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagView create)
    {
        if (create == null)
            return BadRequest(ModelState);

        var tags = _tag.GetAllAsync();
        var checkTag = tags.Select(t => t.Name).Contains(create.Name);

        var mappedTag = CreateTagMapper(create);

        if (checkTag)
        {
            ModelState.AddModelError("","Tag already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if (!_tag.CreateTag(mappedTag))
        {
            ModelState.AddModelError("", "Something went wrong! while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully creted:-)");
    }

    [HttpDelete("delete-tag{id}")]
    public IActionResult DeleteTags([FromBody] int id)
    {
        if (!_tag.DeleteTag(id))
        {
            return NotFound(new { message = "Tag not found" });
        }

        return Ok(new { message = "Tag removed successfully" });
    }

    #region CreateMapper
     private Tag CreateTagMapper(CreateTagView create)
        {
            Tag tag = new Tag()
            {
                Name = create.Name
            };
            return tag;
        }
    #endregion
   
}