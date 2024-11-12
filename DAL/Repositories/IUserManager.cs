using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;

namespace Localizard.DAL.Repositories;

public interface IUserManager
{
    ICollection<User> GetAllUsers();
    Task<User> GetByIdAsync(int id);
    bool UserExists(int id);
    bool CreateUser(User user);
    bool Update(User user);
    bool Delete(User user);
    bool Save();

}