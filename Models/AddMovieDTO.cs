using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class AddMovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int GenreId { get; set; }    
        public DateTime ReleaseDate { get; set; }
        [NotMapped]
        [Required]
        public IFormFile? MoviePoster { get; set; } 
        [Required]
        public int Rating { get; set; }
    }
}
