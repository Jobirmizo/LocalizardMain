using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;
using Localizard.Data.Entites;

namespace Localizard.Domain.Entites;

public class Language
{
    [Key] 
    public int Id { get; set; }
    public string Name { get; set; }
    public string LanguageCode { get; set; }
    [JsonIgnore] 
    public ICollection<ProjectInfo> ProjectInfos { get; set; } = new List<ProjectInfo>();
    [JsonIgnore]
    public virtual ICollection<Translation> Translations { get; set; }
}