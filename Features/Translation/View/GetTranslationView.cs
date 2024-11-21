using Localizard.Domain.Entites;

namespace Localizard.Domain.ViewModel;

public class GetTranslationView
{
    public string Key { get; set; }
    public Language Language { get; set; }
    public string Text { get; set; }
}