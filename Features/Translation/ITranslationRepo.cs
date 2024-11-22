using Localizard.Domain.Entites;

namespace Localizard.DAL.Repositories.Implementations;

public interface ITranslationRepo
{
    ICollection<Translation> GetAll();
    Task<Translation> GetById(int id);
    bool TranslationExists(int id);
    bool CreateTranslation(Translation translation);
    bool UpdateTranslation(Translation translation);
    bool DeleteTranslation(Translation translation);
    bool Save();
}