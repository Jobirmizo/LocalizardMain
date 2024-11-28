using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Localizard.Data.Entites;
using Localizard.Domain.Enums;
using Localizard.Domain.ViewModel;

namespace Localizard.Domain.Entites;

public class ProjectDetail
{
    [Key]
    public int Id { get; set; }
    public string Key { get; set; }
    
    public int TranslationId { get; set; }
    public int ProjectInfoId { get; set; } 
    // public string Description { get; set; }
    public List<int> TagIds { get; set; } = new List<int>();
    public virtual ICollection<Translation> Translation { get; set; }
    [JsonIgnore]
    public List<ProjectInfo> ProjectInfo { get; set; }
    [JsonIgnore]
    public PlatformEnum PlatformCategories { get; set; }
    
}