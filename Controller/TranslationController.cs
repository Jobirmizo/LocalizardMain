using System.Net.Sockets;
using System.Security.Cryptography;
using AutoMapper;
using Localizard.DAL;
using Localizard.DAL.Repositories;
using Localizard.DAL.Repositories.Implementations;
using Localizard.Data.Entites;
using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;
using Localizard.Features.Translation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Localizard.Controller;

[Route("api/[controller]/[action]")]
[ApiController]
public class TranslationController : ControllerBase
{

    private readonly ITranslationRepo _translationRepo;
    private readonly IMapper _mapper;
    private readonly ILanguageRepo _languageRepo;
    
    public TranslationController(IMapper mapper, ITranslationRepo translationRepo, ILanguageRepo languageRepo)
    {
        _mapper = mapper;
        _translationRepo = translationRepo;
        _languageRepo = languageRepo;
    }
    
    
    [HttpGet]
    public IActionResult GetAllTranslations()
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

        var checkTranslation = _translationRepo.GetAll().Select(t => t.SymbolKey).Contains(create.SymbolKey);

        var translation = CreateTranslationMappper(create);
        
        

        if (checkTranslation)
        {
            ModelState.AddModelError("","Language already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_translationRepo.CreateTranslation(translation))
        {
            ModelState.AddModelError("","Something went wrong");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfull created");
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateTranslation(int id, [FromBody] UpdateTranslationView update)
    {
        if (update == null)
            return BadRequest(ModelState);

        var existingTranslation = await _translationRepo.GetById(id);

        if (existingTranslation == null)
            return NotFound("there is no such translation");

        var checkTranslation = _translationRepo.GetAll().Any(t => t.Id == update.Id && t.Id != id);

        if (checkTranslation)
        {
            ModelState.AddModelError("","Translation already exist, check it again!");
            return StatusCode(422, ModelState);
        }

        // existingTranslation.Text = update.Text;
        // existingTranslation.LanguageId = update.LanguageId;

        if (!ModelState.IsValid)
            return BadRequest();

        if (!_translationRepo.UpdateTranslation(existingTranslation))
        {
            ModelState.AddModelError("","Something went wrong while updating the translation");
            return StatusCode(500, ModelState);
        }

        return Ok("updated)");
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
    
    
    #region GetTranslation
       private GetTranslationView GetTranslationMapper(Translation translation)
       {
           GetTranslationView translate = new GetTranslationView()
           {
                SymbolKey = translation.SymbolKey,
                LanguageId = translation.LanguageId,
                Text = translation.Text
           };

           return translate;
       }
    #endregion
    #region CreateTranlation
    private Translation CreateTranslationMappper(CreateTranslationView create)
    {
        Translation translate = new Translation()
        {
            SymbolKey = create.SymbolKey,
            Text = create.Text,
            LanguageId = create.LanguageId
        };
        return translate;
    }
    #endregion
 
}