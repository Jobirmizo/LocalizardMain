using Localizard.DAL.Repositories;
using Localizard.Data.Entites;
using Localizard.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Localizard.Controller;
[Route("api/[controller]/[action]")]
[ApiController]

public class TagController : ControllerBase
{

    private readonly ITagRepo _tag;

    public TagController(ITagRepo tag)
    {
        _tag = tag;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTags()
    {
        var tags = await _tag.GetAllAsync();
        return Ok(tags);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> CreateTag(int id)
    {
        var tag = await _tag.GetById(id);
        return Ok(tag);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagView create)
    {
        if (create == null)
            return BadRequest(ModelState);

        var tags = await _tag.GetAllAsync();
        var checkTag = tags.Select(t => t.Name).Contains(create.Name);

        var mappedTag = CreateTagMapper(create);

        if (checkTag)
        {
            ModelState.AddModelError("","Tag already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!_tag.CreateTag(mappedTag))
        {
            ModelState.AddModelError("", "Something went wrong! while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully creted:-)");
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