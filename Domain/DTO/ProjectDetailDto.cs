using System.Text.Json.Serialization;
using AutoMapper.Configuration.Annotations;
using Localizard.Domain.Entites;

namespace Localizard.Domain.ViewModel;

public class ProjectDetailDto
{
    public string Key { get; set; }
    public int TranslationId { get; set; }
    public string Description { get; set; }
    public string Tag { get; set; }
    
    [JsonIgnore]
    public Translation Translation { get; set; }
}