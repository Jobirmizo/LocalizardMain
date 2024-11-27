using System.Text.Json.Serialization;
using Localizard.Domain.Entites;

namespace Localizard.Domain.ViewModel;

public class GetProjectView
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int DefaultLanguageId { get; set; }
    public int[] AvailableLanguageIds { get; set; } = null!;
    
    [JsonIgnore]
    public  ICollection<ProjectDetail> AvialableProjectDetails { get; set; }
    
}