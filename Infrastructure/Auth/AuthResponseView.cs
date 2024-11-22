namespace Localizard.Domain.ViewModel;

public class AuthResponseView
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }

        public AuthResponse(string token, string username, string role)
        {
            Token = token;
            Username = username;
            Role = role;
        }
    }
}