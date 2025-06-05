using Microsoft.EntityFrameworkCore;
using MovieApplicationApi.Models.Entities;

namespace MovieApplicationApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) :
        base(options)
    {
        
    }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Genre> Genres { get; set; }    
    public DbSet<Movie> Movies { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<BlackListToken> BlackListTokens { get; set; }
}
