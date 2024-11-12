using AutoMapper.Configuration.Annotations;
using Localizard.Domain.Entites;

namespace Localizard.Domain.ViewModel;

public class ProjectDetailDto
{
    public string Key { get; set; }
    public List<TranslationDto> TranslationId { get; set; }
    public string Description { get; set; }
    public string Tag { get; set; }
}