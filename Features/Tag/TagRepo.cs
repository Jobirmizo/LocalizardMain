using Localizard.Domain.Enums;

namespace Localizard.DAL.Repositories;

public class TagRepo : ITagRepo
{
    private readonly List<TagEnum> _tag = Enum.GetValues(typeof(TagEnum)).Cast<TagEnum>().ToList();

    public async Task<IEnumerable<GetTagView>> GetAllTagsAsync()
    {
        return Enum.GetValues(typeof(TagEnum))
            .Cast<TagEnum>()
            .Select(t => new GetTagView()
            {
                Id = (int)t,
                Name = t.ToString()
            });
    }

    public async Task<TagEnum> GetById(int id)
    {
        var tag = _tag.FirstOrDefault(t => (int)t == id);
        return await Task.FromResult(tag);
    }
}