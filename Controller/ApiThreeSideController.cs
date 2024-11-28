using Localizard.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Localizard.Controller;

public class ApiThreeSideController : ControllerBase
{
    private readonly AppDbContext _context;

    public ApiThreeSideController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("get-project")]
    public async Task<IActionResult> GetProjects([FromQuery] string projectName)
    {
        var myEntity = await _context.Projects
            .Where(e => e.Name == projectName)
            .FirstOrDefaultAsync();
        if (myEntity == null)
        {
            return NotFound("No project found the specified projectName");
        }

        var projectDetails = await _context.ProjectDetails
            .Where(p => p.ProjectInfoId == myEntity.Id)
            .Include(p => p.Translation)
            .ToListAsync();

        if (projectDetails == null || !projectDetails.Any())
        {
            return NotFound("No project details found.");
        }

        var result = projectDetails.Select(p => new
        {
            id = p.Id,
            namekeys = p.Key,
            parent = p.ProjectInfoId,
            translations = p.Translation.Select(t => new
            {
                t.ProjectDetails,
            }).ToList()
        }).ToList();

        return Ok(result);
    }
}