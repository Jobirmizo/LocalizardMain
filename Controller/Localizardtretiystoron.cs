using Localizard.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Localizard.Controller;

public class Localizardtretiystoron : ControllerBase
{
    private readonly AppDbContext _context;

    public Localizardtretiystoron(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("Apitretiystoron")]
    public async Task<IActionResult> Getproject([FromQuery] string Name)
    {
        var projectInfo = await _context.Projects
            .Where(p => p.Name == Name)
            .FirstOrDefaultAsync();

        if (projectInfo == null)
        {
            return NotFound("Project not found");
        }

        var projectDetail = await _context.ProjectDetails
            .Where(p => p.TranslationId == projectInfo.Id)
            .Include(p => p.Translation)
            .ToListAsync();

        if (projectDetail == null || !projectDetail.Any())
        {
            return NotFound("No project details found");
        }

        var result = projectDetail.Select(p => new
        {
            id = p.Id,
            name = p.Key,
            translationId = p.TranslationId,
            translation = p.Translation.Languages,
            text = p.Translation.Text,
        }).ToList();
    
        return Ok(result);

    }
   
}