using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Localizard.Domain.Entites;

namespace Localizard.Domain.ViewModel;

public class CreateTranslationView
{
    public int LanguageId { get; set; }
    public string Text { get; set; }
   
}
