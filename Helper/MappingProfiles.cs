using AutoMapper;
using Localizard.DAL.Repositories.Implementations;
using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;

namespace Localizard.Helper;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<User, LoginDto>();
        CreateMap<LoginDto, User>();

        CreateMap<User, RegisterDto>();
        CreateMap<RegisterDto, User>();

        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();
        
        CreateMap<User, GetUsersDto>();
        CreateMap<GetUsersDto, User>();

        CreateMap<Language, LanguageDto>();
        CreateMap<LanguageDto, Language>();
        
        CreateMap<Translation, TranslationDto>();
        CreateMap<TranslationDto, Translation>();
        
        CreateMap<ProjectInfo, ProjectInfoDto>();
        CreateMap<ProjectInfoDto, ProjectInfo>();

        CreateMap<ProjectDetail, ProjectInfoDto>();
        CreateMap<ProjectDetailDto, ProjectDetail>();

        
    }
    
}