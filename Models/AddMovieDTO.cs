using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class AddMovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int GenreId { get; set; }
        public DateTime ReleaseDate { get; set; }
        [NotMapped]
        public IFormFile MoviePoster { get; set; }
        [Required]
        public int Rating { get; set; }
    }
}
