using Microsoft.AspNetCore.Identity;

namespace Localizard.Domain.ViewModel;

public class UserView
{
    
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}