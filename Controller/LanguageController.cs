using AutoMapper;
using Localizard.DAL.Repositories;
using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Localizard.Controller;

[Route("api/languge/")]
[ApiController]
[Authorize]
public class LanguageController : ControllerBase
{
    private readonly ILanguageRepo _languageRepo;
    private readonly IMapper _mapper;
    
    public LanguageController(ILanguageRepo languageRepo, IMapper mapper)
    {
        _languageRepo = languageRepo;
        _mapper = mapper;
    }
    
    [HttpGet("get-all")]
    public IActionResult GetAllLanguages()
    {
        var languages = _languageRepo.GetAll();
        var mappedLanguages = _mapper.Map<List<LanguageView>>(languages);

        if (!ModelState.IsValid)
            return BadRequest();
        
        return Ok(mappedLanguages);
    }

    [HttpGet("get-by/{id}")]
    public async Task<IActionResult> GetLanguageById(int id)
    {
        if (!ModelState.IsValid)
            return NotFound(ModelState);

        var language = await _languageRepo.GetById(id);

        if (!ModelState.IsValid)
            return BadRequest();

        return Ok(language);
    }

    [HttpPost ("create")]
    public IActionResult CreateLanguage([FromBody] LanguageView languageCreate)
    {
        if (languageCreate == null)
            return BadRequest(ModelState);

        var language = _languageRepo.GetAll()
            .Where(l => l.Name.Trim().ToUpper() == languageCreate.Name.TrimEnd().ToUpper())
            .FirstOrDefault();

        if (language != null)
        {
            ModelState.AddModelError("", "Language already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest();

        var languageMap = _mapper.Map<Language>(languageCreate);

        if (!_languageRepo.CteateLanguage(languageMap))
        {
            ModelState.AddModelError("", "Something went wrong");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created;-)");
    }

    [HttpPut ("update")]
    public async Task<IActionResult> UpdateLanguage(int id, [FromBody] UpdateLanguageView update)
    {
        if (update == null)
            return BadRequest(ModelState);

        var checkLanguage = await _languageRepo.GetById(id);

        if (checkLanguage == null)
            return NotFound($"Language alreadyExist");

        var languageExists = _languageRepo.GetAll().Any(l => l.LanguageCode == update.LanguageCode && l.Id != id);

      
        if (languageExists)
        {
            ModelState.AddModelError("","Same language already exists!");
            return StatusCode(422, ModelState);
        }

        checkLanguage.Name = update.Name;
        checkLanguage.LanguageCode = update.LanguageCode;
        
        
        if (!ModelState.IsValid)
            return BadRequest();

        if (!_languageRepo.UpdateLanguage(checkLanguage))
        {
            ModelState.AddModelError("","Something went wrong while saving Language");
            return StatusCode(500, ModelState);
        }

        return Ok("Updated");
    }

    [HttpDelete("delete/{id:int}")]
    public IActionResult DeleteLanguage(int id)
    {
        
        if (!_languageRepo.DeleteLanguage(id))
        {
            return NotFound(new { message = "Language not found or could not be deleted" });
        }

        return Ok(new { message = "Language deleted successfully" });
    }
    
    
    
}