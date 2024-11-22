using System.ComponentModel.DataAnnotations;

namespace Localizard.Domain.ViewModel;

public class LoginResponseView
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string? AccessToken { get; set; }
    public int ExpiresIn { get; set; }
}