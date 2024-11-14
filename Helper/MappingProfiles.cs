using AutoMapper;
using Localizard.DAL.Repositories.Implementations;
using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        CreateMap<ProjectInfo, CreateProjectView>();
        CreateMap<CreateProjectView, ProjectInfo>();

        CreateMap<ProjectDetail, ProjectDetailDto>();
        CreateMap<ProjectDetailDto, ProjectDetail>();

        CreateMap<ProjectInfo, UpdateProjectDto>();
        CreateMap<UpdateProjectDto, ProjectInfo>();

        CreateMap<UpdateProjectDetailDto, ProjectDetail>();
        CreateMap<ProjectDetail, UpdateProjectDetailDto>();

        CreateMap<UpdateProjectDto, Language>();
        CreateMap<Language, UpdateLanguageDto>();
        
    }
}