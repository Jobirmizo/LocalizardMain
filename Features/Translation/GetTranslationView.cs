using Localizard.Domain.Entites;

namespace Localizard.Domain.ViewModel;

public class GetTranslationView
{   
    public LanguageView Language { get; set; }
    public string Text { get; set; }
}