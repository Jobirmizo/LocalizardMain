using Localizard.Data.Entites;
using Localizard.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Localizard.DAL.Repositories;

public class TagRepo : ITagRepo
{

    private readonly AppDbContext _context;

    public TagRepo(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Tag>> GetAllAsync()
    {
        return await _context.Tags.OrderBy(t => t.Id).ToListAsync();
    }

    public async Task<Tag> GetById(int id)
    {
        return await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
    }

    public bool TagExists(int id)
    {
        return _context.Tags.Any(t => t.Id == id);
    }

    public bool CreateTag(Tag tag)
    {
        _context.Add(tag);
        return Save();
    }
    
    public bool Save()
    {
        var saved = _context.SaveChanges();
        return saved > 0 ? true : false;
    }
}