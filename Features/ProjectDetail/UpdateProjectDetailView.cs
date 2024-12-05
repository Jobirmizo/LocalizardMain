using Localizard.Data.Entites;
using Localizard.Features.Translation;

namespace Localizard.Domain.ViewModel;

public class UpdateProjectDetailView
{
    public int Id { get; set; }
    public string Key { get; set; }
    public ICollection<UpdateTranslationView> Translations { get; set; }
    public string Description { get; set; }
    
    public virtual ICollection<Tag> Tags { get; set; }
}