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

[Route("api/translation")]
[ApiController]
[Authorize]
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

    
    [AllowAnonymous]
    [HttpGet("get-by/{id}")]
    public async Task<IActionResult> GetTranslationById(int id)
    {
        if (!_translationRepo.TranslationExists(id))
            return NotFound();

        var translation = await _translationRepo.GetById(id);

        if (!ModelState.IsValid)
            return BadRequest();

        return Ok(translation);
    }
}

    
 