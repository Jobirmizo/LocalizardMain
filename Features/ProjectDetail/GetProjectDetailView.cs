using System.Text.Json.Serialization;
using Localizard.Domain.Entites;

namespace Localizard.Domain.ViewModel;

public class GetProjectDetailView
{
    public string Key { get; set; }
    public string Description { get; set; }
    public string Tag { get; set; }
    public int ProjectInfoId { get; set; }
    public ICollection<Translation> AvailableTranslations { get; set; }
    
    [JsonIgnore]
    public ProjectInfo ProjectInfo { get; set; }
}