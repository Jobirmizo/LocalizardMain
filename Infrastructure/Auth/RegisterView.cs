using System.ComponentModel.DataAnnotations;

namespace Localizard.Domain.ViewModel;

public class RegisterView
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; }
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }

    public string Role { get; set; }
}