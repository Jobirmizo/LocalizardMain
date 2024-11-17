using System.ComponentModel.DataAnnotations;

namespace Localizard.Domain.ViewModel;

public class LoginView
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; }
    public string Password { get; set; }
}