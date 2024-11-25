using Localizard.Domain.Entites;

namespace Localizard.Domain.ViewModel;

public class GetProjectView
{
    public string Name { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Language DefaultLanguage { get; set; } 
    public ICollection<Language> AvialableLanguages { get; set; } = null!;
    public  ICollection<ProjectDetail> AvialableProjectDetails { get; set; }
    
}