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
    private readonly IMapper _mapper;
    
    public TranslationController(IMapper mapper, ITranslationRepo translationRepo)
    {
        _mapper = mapper;
        _translationRepo = translationRepo;
    }
    
    
    [HttpGet]
    public async Task<IActionResult> GetAllTranslations()
    {
        var translations = _translationRepo.GetAll();

        var translationView = new List<GetTranslationView>();
        foreach (var tranlate in translations)
        {
            var translateView = GetTranslationMapper(tranlate);
            translationView.Add(translateView);
        }

        return Ok(translationView);
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
    public IActionResult CreateTranslation([FromBody] CreateTranslationView create)
    {

        if (create == null)
            return BadRequest(ModelState);

        var translation = _translationRepo.GetAll()
            .Where(l => l.Key.Trim().ToUpper() == create.Key.TrimEnd().ToUpper())
            .FirstOrDefault();

        if (translation != null)
        {
            ModelState.AddModelError("", "Language already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var mappedtranslation = _mapper.Map<Translation>(create);

        if (!_translationRepo.CreateTranslation(mappedtranslation))
        {
            ModelState.AddModelError("", "Something went wrong");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfull created;-)");
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


    private GetTranslationView GetTranslationMapper(Translation translation)
    {
        GetTranslationView getTranslates = new GetTranslationView()
        {
            Key = translation.Key,
            Text = translation.Text,
            Language = new LanguageView()
            {
                Name = translation.Language.Name,
                LanguageCode = translation.Language.LanguageCode
            }
        };

        return getTranslates;
    }
}