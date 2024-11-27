namespace Localizard.Features.Translation;

public class UpdateTranslationView
{
    public string SymbolKey { get; set; }
    public int Id { get; set; }
    public int LanguageId { get; set; }
    public string Text { get; set; }
}