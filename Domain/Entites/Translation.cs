using System.ComponentModel.DataAnnotations;

namespace Localizard.Domain.Entites;

public class Translation
{
    [Key] 
    public int Id { get; set; }
    public string Key { get; set; }
    public string Language { get; set; }
    public string Text { get; set; }

}