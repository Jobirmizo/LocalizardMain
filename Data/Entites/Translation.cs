using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Localizard.Domain.ViewModel;

namespace Localizard.Domain.Entites;

public class  Translation
{
    [Key] 
    public int Id { get; set; }
    public int LanguageId { get; set; }
    public string Text { get; set; }
    public virtual Language Language { get; set; }
    [JsonIgnore]
    public virtual ICollection<ProjectDetail> ProjectDetails { get; set; } = new List<ProjectDetail>();

}