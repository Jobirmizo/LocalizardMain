using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Localizard.DAL.Repositories.Implementations;

public class ProjectRepo : IProjectRepo
{
    private readonly AppDbContext _context;
    
    public ProjectRepo(AppDbContext context)
    {
        _context = context;
    }

    public  ICollection<ProjectInfo> GetAllProjects()
    {
        return _context.Projects.Include(x=>x.Languages).OrderBy(p => p.Id).ToList();
    }

    public async Task<ProjectInfo> GetById(int id)
    {
        return await _context.Projects.Include(l => l.Languages).FirstOrDefaultAsync(p => p.Id == id);
    }

    public bool ProjectExists(int id)
    {
        return _context.Projects.Any(p => p.Id == id);
    }

    public bool CreateProject(ProjectInfo projectInfo)
    {
        _context.Add(projectInfo);
        return Save();
    }

    public bool UpdateProject(ProjectInfo project)
    {
        _context.Update(project);
        return Save();
    }

    public bool DeleteProject(int id)
    {
        var project = _context.Projects.FirstOrDefault(p => p.Id == id);
        
        if(project != null)
            _context.Projects.Remove(project);

        return Save();
    }
    

    public bool Save()
    {
        var saved = _context.SaveChanges();
        return saved > 0 ? true : false;
    }
}