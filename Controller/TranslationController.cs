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

    
 
 


}