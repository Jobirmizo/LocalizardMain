using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Localizard.DAL.Repositories.Implementations;

public class UserManager : IUserManager
{
    private readonly AppDbContext _context;
    
    public UserManager(AppDbContext context)
    {
        _context = context;
    }

    public ICollection<User> GetAllUsers()
    {
        return _context.Users.OrderBy(p => p.Id).ToList();
    }

    public async Task<User> GetByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(i => i.Id == id);
    }
    
    public bool UserExists(int id)
    {
        return _context.Users.Any(p => p.Id == id);
    }

    public bool CreateUser(User user)
    {
        _context.Add(user);
        return Save();
    }
    public bool Update(User user)
    {
        _context.Update(user);
        return Save();
    }

    public bool Delete(User user)
    {
        _context.Remove(user);
        return Save();
    }

    public bool Save()
    {
        var saved = _context.SaveChanges();
        return saved > 0 ? true : false;
    }
}