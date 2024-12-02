using System.Text.Json.Serialization;
using AutoMapper.Configuration.Annotations;
using Localizard.Domain.Entites;

namespace Localizard.Domain.ViewModel;

public class CreateProjectDetailView
{
    public string Key { get; set; }
    public int ProjectInfoId { get; set; }
    
    public string Description { get; set; }
    public int[] TagIds { get; set; } = null!;
    public List<CreateTranslationView> Translations { get; set; }
}