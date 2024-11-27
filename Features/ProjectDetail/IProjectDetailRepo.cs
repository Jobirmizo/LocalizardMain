using Localizard.Domain.Entites;

namespace Localizard.DAL.Repositories;

public interface IProjectDetailRepo
{
    ICollection<ProjectDetail> GetAll();
    Task<ProjectDetail> GetById(int id);
    bool ProjectDetailExist(int id);
    bool CreateProjectDetail(ProjectDetail projectDetail);
    bool UpdateProjectDetail(ProjectDetail projectDetail);
    bool DeleteProjectDetail(int id);
    bool Save();
}