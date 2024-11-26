using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;
using Localizard.Domain.Enums;
using Localizard.Domain.ViewModel;

namespace Localizard.Domain.Entites;

public class ProjectInfo
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public int LanguageId { get; set; }
    public string CreatedBy { get; set; }
    [JsonIgnore]
    public DateTime? CreatedAt { get; set; }
    [JsonIgnore]
    public DateTime? UpdatedAt { get; set; }
    
    public virtual ICollection<ProjectDetail> ProjectDetail { get; set; }
    public virtual ICollection<Language> Languages { get; set; }
}