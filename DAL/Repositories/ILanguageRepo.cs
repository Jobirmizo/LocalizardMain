using Localizard.Domain.Entites;
using Localizard.Domain.Enums;
using Localizard.Domain.ViewModel;

namespace Localizard.DAL.Repositories;

public interface ILanguageRepo
{
    List<Language> GetAll();
    Task<Language> GetById(int id);
    bool LanguageExists(int id);
    bool CteateLanguage(Language language);
    bool Update(User user);
    bool Delete(User user);
    bool Save();
}