using System.Text.Json.Serialization;
using Localizard.Domain.Entites;

namespace Localizard.Data.Entites;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    [JsonIgnore] 
    public virtual ICollection<ProjectDetail> ProjectDetails { get; set; } = new List<ProjectDetail>();
}