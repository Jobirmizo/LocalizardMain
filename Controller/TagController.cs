using Localizard.DAL.Repositories;
using Localizard.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

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
        var tags = await _tag.GetAllTagsAsync();
        return Ok(tags);
    }
}