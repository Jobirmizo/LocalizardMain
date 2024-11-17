using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Localizard.Domain.Entites;

public class User
{
    [Key] 
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}