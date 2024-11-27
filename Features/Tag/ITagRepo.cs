using Localizard.Domain.Enums;

namespace Localizard.DAL.Repositories;

public interface ITagRepo
{
    Task<IEnumerable<GetTagView>> GetAllTagsAsync();
    Task<TagEnum> GetById(int id);
}