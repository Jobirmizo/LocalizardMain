using System.Net;
using System.Text.Json.Serialization;
using Localizard.DAL.Repositories;
using Localizard.Data.Entites;
using Localizard.Domain.Entites;
using Localizard.Domain.Enums;

namespace Localizard.Domain.ViewModel;

public class GetProjectDetailView
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Description { get; set; }
    public int ProjectInfoId { get; set; }
    public int[] TagIds { get; set; }
    public ICollection<Tag> Tags { get; set; }
    public ICollection<Translation> AvailableTranslations { get; set; }
    
    [JsonIgnore]
    public ProjectInfo ProjectInfo { get; set; }
}