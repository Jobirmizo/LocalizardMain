using System.Net.Sockets;
using System.Security.Cryptography;
using AutoMapper;
using Localizard.DAL;
using Localizard.DAL.Repositories;
using Localizard.DAL.Repositories.Implementations;
using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Localizard.Controller;

[Route("api/[controller]/[action]")]
[ApiController]
public class TranslationController : ControllerBase
{

    private readonly ITranslationRepo _translationRepo;
    private readonly ILanguageRepo _languageRepo;
    private readonly IMapper _mapper;
    
    public TranslationController(IMapper mapper, ITranslationRepo translationRepo, ILanguageRepo languageRepo)
    {
        _mapper = mapper;
        _translationRepo = translationRepo;
        _languageRepo = languageRepo;
    }

    private Translation TranslationMapper(CreateTranslationView createTranslation)
    {
        Translation create = new Translation()
        {
            Key = createTranslation.Key,
            LanguageId = createTranslation.LanguageId,
            Text = createTranslation.Text
        };
        return create;
    }

    [HttpGet]
    public IActionResult GetAllTranslations()
    {
        var translations = _translationRepo.GetAll();
        var mappedTranslations = _mapper.Map<List<GetTranslationView>>(translations);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(mappedTranslations);
    }
    
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTranslationById(int id)
    {
        if (!_translationRepo.TranslationExists(id))
            return NotFound();

        var translation = await _translationRepo.GetById(id);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(translation);
    }
    
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public  IActionResult CreateTranslation([FromBody] CreateTranslationView create)
    {
        var translate = _translationRepo.GetAll().Select(x => x.Key).Contains(create.Key);
        var translation = TranslationMapper(create);

        var translationLanguageId = _languageRepo.GetById(create.LanguageId);
        if (translation != null)
        {
            translation.LanguageId = translationLanguageId.Id;
        }
        
        if (translate)
        {
            ModelState.AddModelError("","Translation key is already used");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var mappedTranslation = _mapper.Map<Translation>(create);

        if (_translationRepo.CreateTranslation(mappedTranslation))
        {
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Success");
    }
    
    [HttpPut]
    public IActionResult UpdateTranslation(int translationId, [FromBody] Translation updateTranslation)
    {
        if (updateTranslation == null)
            return BadRequest(ModelState);

        if (translationId != updateTranslation.Id)
            return BadRequest(ModelState);

        if (!_translationRepo.TranslationExists(translationId))
            return NotFound();

        if (!ModelState.IsValid)
            return BadRequest();
        if (!_translationRepo.UpdateTranslation(updateTranslation))
        {
            ModelState.AddModelError("", "Something went wrong while updating the Translation");
            return StatusCode(500, ModelState);
        }

        return NoContent();

    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTranslation(int translationId)
    {
        if (!_translationRepo.TranslationExists(translationId))
            return NotFound();

        var translationToDelete = await _translationRepo.GetById(translationId);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_translationRepo.DeleteTranslation(translationToDelete))
        {
            ModelState.AddModelError("", "Something went wrong while deleting translation");
        }

        return NoContent();
    }
    
}