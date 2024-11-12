using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;

namespace Localizard.DAL.Repositories;

public interface IProjectRepo
{
    ICollection<ProjectInfo> GetAllProjects();
    Task<ProjectInfo> GetById(int id);
    bool ProjectExists(int id);
    bool CreateProject(ProjectInfo projectInfo);
    bool Save();
}