namespace Localizard.Domain.ViewModel;

public class UpdateProjectView
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int DefaultLanguageId { get; set; } 
    public int ProjectDetailId { get; set; }
    public int[] AvailableLanguageIds { get; set; } = null!;
    public List<LanguageView> AvailableLanguageId { get; set; } = new List<LanguageView>();
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}