using Localizard.Data.Entites;
using Localizard.Domain.Enums;

namespace Localizard.DAL.Repositories;

public interface ITagRepo
{
    List<Tag> GetAllAsync();
    Task<Tag> GetById(int id);
    bool TagExists(int id);
    bool CreateTag(Tag tag);
    bool Save();
}