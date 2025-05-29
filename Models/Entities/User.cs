using MovieApplicationApi.Models.Entities;

namespace WebApplication1.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Roles { get; set; }

    public ICollection<RefreshToken>? RefreshTokens { get; set; } //It is a navigation property
}
