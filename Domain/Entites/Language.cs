using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace Localizard.Domain.Entites;

public class Language
{
    [Key] 
    public int Id { get; set; }
    public string Name { get; set; }
    public string LanguageCode { get; set; }
}