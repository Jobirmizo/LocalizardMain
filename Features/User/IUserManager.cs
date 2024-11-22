using Localizard.Domain.Entites;
using Localizard.Domain.ViewModel;

namespace Localizard.DAL.Repositories;

public interface IUserManager
{
    ICollection<User> GetAllUsers();
    Task<User> GetByIdAsync(int id);
    bool UserExists(int id);
    bool CreateUser(User user);
    bool UpdateUser(User user);
    bool DeleteUser(User user);
    bool Save();

}