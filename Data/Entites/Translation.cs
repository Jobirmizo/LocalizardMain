using System.ComponentModel.DataAnnotations;
using Localizard.Domain.ViewModel;

namespace Localizard.Domain.Entites;

public class  Translation
{
    [Key] 
    public int Id { get; set; }
    public string Key { get; set; }
    public int LanguageId { get; set; }
    public string Text { get; set; }
    public Language Languages { get; set; } = null!;

}