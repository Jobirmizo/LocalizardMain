using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Localizard.Domain.Entites;
using Localizard.Domain.Enums;

namespace Localizard.DAL.Repositories;

public class GetTagView
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    [JsonIgnore]
    public ICollection<ProjectDetail> ProjectDetails { get; set; } = new List<ProjectDetail>();
}