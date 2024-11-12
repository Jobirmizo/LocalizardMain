using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Localizard.Domain.Enums;

namespace Localizard.Domain.Entites;

public class ProjectDetail
{
    [Key]
    public int Id { get; set; }
    public string Key { get; set; }
    public List<Translation> TranslationId { get; set; }
    public string Description { get; set; }
    public string Tag { get; set; }
    public virtual Translation Translation { get; set; }
    public PlatformEnum PlatformCategories { get; set; }
    
}