using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Localizard.DAL.Repositories;
using Localizard.Data.Entites;
using Localizard.Domain.Enums;
using Localizard.Domain.ViewModel;

namespace Localizard.Domain.Entites;

public class ProjectDetail
{
    [Key]
    public int Id { get; set; }
    public string Key { get; set; }
    public string Description { get; set; }
    public int ProjectInfoId { get; set; }
    public int[] TagIds { get; set; }
    public virtual ICollection<Translation> Translations { get; set; }
    [JsonIgnore]
    public ProjectInfo ProjectInfo { get; set; }
    [JsonIgnore]
    public PlatformEnum PlatformCategories { get; set; }
    [JsonIgnore] 
    public virtual ICollection<Tag> Tags { get; set; }
}