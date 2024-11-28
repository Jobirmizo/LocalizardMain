using System.Text.Json.Serialization;
using Localizard.DAL.Repositories;
using Localizard.Data.Entites;
using Localizard.Domain.Entites;
using Localizard.Domain.Enums;

namespace Localizard.Domain.ViewModel;

public class GetProjectDetailView
{
    public string Key { get; set; }
    public string Description { get; set; }
    public int ProjectInfoId { get; set; }
    public ICollection<GetTagView> Tags { get; set; }
    public ICollection<Translation> AvailableTranslations { get; set; }
    
    [JsonIgnore]
    public ProjectInfo ProjectInfo { get; set; }
}