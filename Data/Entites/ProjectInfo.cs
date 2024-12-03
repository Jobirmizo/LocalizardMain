using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;
using Localizard.Data.Entites;
using Localizard.Domain.Enums;
using Localizard.Domain.ViewModel;
using Localizard.Features.Auditable;

namespace Localizard.Domain.Entites;

public class ProjectInfo : AuditableEntity
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public int LanguageId { get; set; }
    public string CreatedBy { get; set; }
    public virtual ICollection<Language> Languages { get; set; }
    [JsonIgnore]
    public virtual List<ProjectDetail> ProjectDetail { get; set; } = new List<ProjectDetail>();
  
}