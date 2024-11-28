using System.Text.Json.Serialization;
using AutoMapper.Configuration.Annotations;
using Localizard.Domain.Entites;
using Localizard.Domain.Enums;

namespace Localizard.Domain.ViewModel;

public class CreateProjectView
{
    public string Name { get; set; }
    public int DefaultLanguageId { get; set; }
    public int[] AvailableLanguageIds { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
}