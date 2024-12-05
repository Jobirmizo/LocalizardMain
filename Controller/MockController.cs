using Bogus;
using Localizard.DAL;
using Localizard.Data.Entites;
using Localizard.Domain.Entites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Localizard.Controller;
[Route("api/mock/")]
[ApiController]

public class MockController : ControllerBase
{

    private readonly AppDbContext _context;
    
    public MockController(AppDbContext context)
    {
        _context = context;
    }
  
    public static List<ProjectInfo> GenerateProjectInfos(int numberOfProjects, List<Language> languages)
    {
        var faker = new Faker<ProjectInfo>()
            .RuleFor(p => p.Name, f => f.Company.CompanyName())
            .RuleFor(p => p.LanguageId, f => f.PickRandom(languages).Id)
            .RuleFor(p => p.CreatedBy, f => f.PickRandom(new[] { "Admin", "User" }));
        

        return faker.Generate(numberOfProjects);
    }
  
    public static List<ProjectDetail> GenerateProjectDetail(int numberOfDetails, List<ProjectInfo> projectInfos, List<Tag> tags)
    {
        var faker = new Faker<ProjectDetail>()
            .RuleFor(pd => pd.Key, f => f.Lorem.Word())
            .RuleFor(pd => pd.Description, f => f.Lorem.Sentence())
            .RuleFor(pd => pd.ProjectInfoId, f => f.PickRandom(projectInfos).Id)
            .RuleFor(pd => pd.TagIds, f => f.PickRandom(tags, 3).Select(t => t.Id).ToArray());
        
        return faker.Generate(numberOfDetails);
    }
    
    
    public static List<Translation> GenerateTranslations(int numberOfTranslations, List<Language> languages, List<ProjectDetail> projectDetails)
    {
        var faker = new Faker<Translation>()
            .RuleFor(t => t.SymbolKey, f => f.Lorem.Word())
            .RuleFor(t => t.LanguageId, f => f.PickRandom(languages).Id)
            .RuleFor(t => t.Text, f => f.Lorem.Sentence())
            .RuleFor(t => t.ProjectDetails, f => f.PickRandom(projectDetails));
        

        return faker.Generate(numberOfTranslations); 
    }

    [HttpPost("create-mock-data")]
    public async Task<IActionResult> CreateMockData()
    {
        int numberOfProjects = 1000;
        int numberOfDetails = 100000;
        int numberOfTranslations = 300000;
        
        var languages = await _context.Languages.ToListAsync();
        var tags = await _context.Tags.ToListAsync();
        
        var projectInfos = GenerateProjectInfos(numberOfProjects, languages);
        await _context.Projects.AddRangeAsync(projectInfos);
        await _context.SaveChangesAsync();
        
        var projectDetails = GenerateProjectDetail(numberOfDetails, projectInfos, tags);
        await _context.ProjectDetails.AddRangeAsync(projectDetails);
        await _context.SaveChangesAsync();
        
        var translations = GenerateTranslations(numberOfTranslations, languages, projectDetails);
        await _context.Translations.AddRangeAsync(translations);
        await _context.SaveChangesAsync();
        
    
       
        
        
        
        return Ok(new { Message = "Mock data created successfully!" });
    }


    [HttpDelete]
    public async Task DeleteAllProjectDetailsAsync()
    {
        var allRecordedProjects = _context.Projects;
        var allRecordedTranslation = _context.Translations;
        var allRecordedProjectDetails = _context.ProjectDetails;


        if (await allRecordedProjects.AnyAsync() && await allRecordedTranslation.AnyAsync() &&
            await allRecordedProjectDetails.AnyAsync())
        {
            _context.Translations.RemoveRange(allRecordedTranslation);
            _context.ProjectDetails.RemoveRange(allRecordedProjectDetails);
            _context.Projects.RemoveRange(allRecordedProjects);


            await _context.SaveChangesAsync();
        }

    }
}