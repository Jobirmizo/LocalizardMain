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
    public IActionResult CreateTranslation([FromBody] CreateTranslationView create)
    {
        if (create == null)
            return BadRequest(ModelState);

        var translation = _translationRepo.GetAll()
            .Where(p => p.Key.Trim().ToUpper() == create.Key.TrimEnd().ToUpper())
            .FirstOrDefault();

        if (translation != null)
        {
            ModelState.AddModelError("", "Project already exist!");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var mappedTranslation = _mapper.Map<Translation>(create);

        if (!_translationRepo.CreateTranslation(mappedTranslation))
        {
            ModelState.AddModelError("", "Something went wrong! while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
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