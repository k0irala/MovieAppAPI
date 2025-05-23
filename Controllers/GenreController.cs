using System.Net.WebSockets;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models.Entities;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController(ApplicationDbContext dbContext, IDapperRepository repository) : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Genre> getAllGenres()
        {
            DynamicParameters parameters = new DynamicParameters();
            var allGenres = repository.Query<Genre>("GetAllGenres", parameters);
            return allGenres;
        }
        [HttpGet("{id}")]
        public Genre getGenreById(int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("genreId", id);
            var genre = repository.QuerySingleOrDefault<Genre>("GetGenreById", parameters);
            return genre;
        }
        [HttpPost]
        public IActionResult addGenre(Genre genre)
        {
            var existingGenreName = dbContext.Genres.SingleOrDefault(x => x.GenreName == genre.GenreName);
            if (existingGenreName == null)
            {
                dbContext.Genres.Add(genre);
                dbContext.SaveChanges();
                return Ok(genre);
            }
            return Conflict(new { error = "Genre Already Exists" });
        }
        [HttpPut("{id}")]
        public IActionResult updateGenre(int id, Genre genre)
        {
            var existingGenre = dbContext.Genres.SingleOrDefault(x => x.Id == id);
            if (existingGenre == null)
            {
                return NotFound();
            }
            var duplicateCheck = dbContext.Genres.SingleOrDefault(x => x.GenreName == genre.GenreName);
            if (duplicateCheck == null)
            {
                existingGenre.GenreName = genre.GenreName;
                dbContext.Genres.Update(existingGenre);
                dbContext.SaveChanges();
                return Ok(existingGenre);
            }
            return Conflict(new { error = "Genre already exists" });
        }
        [HttpDelete("{id}")]
        public IActionResult deleteGenre(int id)
        {
            var existingGenre = dbContext.Genres.SingleOrDefault(y => y.Id == id);
            if(existingGenre == null)
            {
                return NotFound();
            }
            dbContext.Genres.Remove(existingGenre);
            dbContext.SaveChanges();
            return Ok(existingGenre);
        }
    }
}
