using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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
    public string Description { get; set; }
    public string Tag { get; set; }
    public virtual ICollection<Translation> Translation { get; set; }
    [JsonIgnore]
    public ICollection<ProjectInfo> ProjectInfos { get; set; } = new List<ProjectInfo>();
    public PlatformEnum PlatformCategories { get; set; }
    
}