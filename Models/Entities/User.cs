
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApplicationApi.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Roles { get; set; }

    [NotMapped]
    public ICollection<RefreshToken>? RefreshTokens { get; set; } //It is a navigation property
}
