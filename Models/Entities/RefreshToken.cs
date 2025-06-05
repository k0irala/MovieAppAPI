using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.Entities;

namespace MovieApplicationApi.Models.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public int userId { get; set; }
    public string RefreshUserToken { get; set; } = string.Empty;

    [NotMapped]
    public User? user { get; set; }
}
