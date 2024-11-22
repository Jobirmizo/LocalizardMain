﻿using AutoMapper;
using Localizard.DAL.Repositories;
using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Localizard.Controller;

[Route("api/[controller]/[action]")]
[ApiController]
// [Authorize]
public class LanguageController : ControllerBase
{
    private readonly ILanguageRepo _languageRepo;
    private readonly IMapper _mapper;
    
    public LanguageController(ILanguageRepo languageRepo, IMapper mapper)
    {
        _languageRepo = languageRepo;
        _mapper = mapper;
    }
    
    [HttpGet]
    public IActionResult GetAllLanguages()
    {
        var languages = _languageRepo.GetAll();
        var mappedLanguages = _mapper.Map<List<LanguageView>>(languages);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        return Ok(mappedLanguages);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLanguageById(int id)
    {
        if (!ModelState.IsValid)
            return NotFound(ModelState);

        var language = await _languageRepo.GetById(id);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(language);
    }

    [HttpPost]
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
            return BadRequest(ModelState);

        var languageMap = _mapper.Map<Language>(languageCreate);

        if (!_languageRepo.CteateLanguage(languageMap))
        {
            ModelState.AddModelError("", "Something went wrong");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfull created;-)");
    }

    [HttpPut]
    public IActionResult UpdateLanguage(int languageId, [FromBody] UpdateLanguageView updateLanguage)
    {
        if (updateLanguage == null)
            return BadRequest(ModelState);

        if (languageId != updateLanguage.Id)
            return BadRequest(ModelState);

        if (!_languageRepo.LanguageExists(languageId))
            return NotFound();

        if (!ModelState.IsValid)
            return BadRequest();

        var languageMap = _mapper.Map<Language>(updateLanguage);

        if (!_languageRepo.UpdateLanguage(languageMap))
        {
            ModelState.AddModelError("","Something went wrong while creating language");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
    
    
}