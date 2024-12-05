using Localizard.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace Localizard.DAL.Repositories.Implementations;

public class ProjectDetailRepo : IProjectDetailRepo
{

    private readonly AppDbContext _context;
    public ProjectDetailRepo(AppDbContext context)
    {
        _context = context;
    }
    public ICollection<ProjectDetail> GetAll()
    {
        return _context.ProjectDetails.Include(x => x.Translations).Include(t => t.Tags).OrderBy(p => p.Id).ToList();
    }

    public async Task<ProjectDetail> GetById(int id)
    {
        return await _context.ProjectDetails.Include(x => x.Translations).Include(x=>x.Tags).FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public bool ProjectDetailExist(int id)
    {
        return _context.ProjectDetails.Any(p => p.Id == id);
    }

    public bool CreateProjectDetail(ProjectDetail projectDetail)
    {
        _context.Add(projectDetail);
        return Save();
    }

    public bool UpdateProjectDetail(ProjectDetail project)
    {
        _context.Update(project);
        return Save();
    }

    public bool DeleteProjectDetail(int id)
    {
        var detail = _context.ProjectDetails.Include(t => t.Translations).FirstOrDefault(d => d.Id == id);

        if (detail == null)
        {
            return false;
        }

        if (detail.Translations != null)
        {
            _context.RemoveRange(detail.Translations);
        }
        
        _context.ProjectDetails.Remove(detail);
        
        return Save();
    }

    public bool Save()
    {
        var saved = _context.SaveChanges();
        return saved > 0 ? true : false;
    }
}