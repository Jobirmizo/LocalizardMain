using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Localizard.Domain.Entites;

namespace Localizard.Domain.ViewModel;

public class CreateTranslationView
{
    public string SymbolKey { get; set; }
    public int LanguageId { get; set; }
    public string Text { get; set; }
   
}
