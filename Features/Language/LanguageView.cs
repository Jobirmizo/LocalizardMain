namespace Localizard.Domain.ViewModel;

public class LanguageView
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string LanguageCode { get; set; }
    public string[] Plurals { get; set; }
}