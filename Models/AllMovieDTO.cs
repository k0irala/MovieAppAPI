using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApplicationApi.Models
{
    public class AllMovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string GenreName { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public string MovieFileName { get; set; } = string.Empty;
        public string MovieFilePath { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
}
