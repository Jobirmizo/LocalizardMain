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
        CreateMap<User, LoginView>();
        CreateMap<LoginView, User>();

        CreateMap<User, RegisterView>();
        CreateMap<RegisterView, User>();

        CreateMap<User, UserView>();
        CreateMap<UserView, User>();

        CreateMap<User, GetUsersView>();
        CreateMap<GetUsersView, User>();

        CreateMap<Language, LanguageView>();
        CreateMap<LanguageView, Language>();

        CreateMap<Translation, CreateTranslationView>();
        CreateMap<CreateTranslationView, Translation>();

        CreateMap<ProjectInfo, CreateProjectView>();
        CreateMap<CreateProjectView, ProjectInfo>();

        CreateMap<ProjectDetail, ProjectDetailView>();
        CreateMap<ProjectDetailView, ProjectDetail>();

        CreateMap<ProjectInfo, UpdateProjectView>();
        CreateMap<UpdateProjectView, ProjectInfo>();

        CreateMap<UpdateProjectDetailView, ProjectDetail>();
        CreateMap<ProjectDetail, UpdateProjectDetailView>();

        CreateMap<UpdateProjectView, Language>();
        CreateMap<Language, UpdateLanguageView>();

        CreateMap<Translation, GetTranslationView>();
        CreateMap<GetTranslationView, Translation>();
        
        CreateMap<Translation, CreateTranslationView>();
        CreateMap<CreateTranslationView, Translation>();

    }
}