using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Build.Construction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models.Entities;
using System.IO;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController(IDapperRepository repository, ApplicationDbContext dbContext) : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Movie> getAllMovies()
        {
            DynamicParameters parameters = new();
            var allMovies = repository.Query<Movie>("GetAllMovies", parameters);
            return allMovies;
        }
        [HttpGet("{id}")]
        public Movie GetMovieById(int id)
        {
            DynamicParameters parameters = new();
            parameters.Add("movieId", id);
            var movie = repository.QuerySingleOrDefault<Movie>("GetMovieById", parameters);
            return movie;
        }
        [HttpPost]
        public async Task<IActionResult> addMovie(AddMovieDTO movie)
        {
            if (movie.MoviePoster != null && movie.MoviePoster.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(movie.MoviePoster.FileName);
                var fullPath = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/images", fileName);

                var directory = Path.GetDirectoryName(fullPath);

                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await movie.MoviePoster.CopyToAsync(stream);
                }

                var posterImagePath = "images/" + fullPath;
                var posterName = movie.MoviePoster.FileName;

                var movieData = new Movie
                {
                    Title = movie.Title,
                    GenreId = movie.GenreId,
                    Rating = movie.Rating,
                    ReleaseDate = movie.ReleaseDate,
                    MovieFileName = posterName,
                    MovieFilePath = posterImagePath
                };

                dbContext.Movies.Add(movieData);
                dbContext.SaveChanges();
                return Ok(movieData);

            }
            return Conflict(new { error = "Failed to add movie to database" });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> updateMovie(int id, AddMovieDTO movie)
        {

            var existingMovie = await dbContext.Movies.SingleOrDefaultAsync(x => x.Id == id);

            if (existingMovie == null)
            {
                return NotFound();
            }
            else
            {
                existingMovie.Title = movie.Title;
                existingMovie.ReleaseDate = movie.ReleaseDate;
                existingMovie.Rating = movie.Rating;
                existingMovie.GenreId = movie.GenreId;
                if (movie.MoviePoster != null && movie.MoviePoster.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(movie.MoviePoster.FileName);
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/images", fileName);

                    var directory = Path.GetDirectoryName(fullPath);

                    if (!string.IsNullOrEmpty(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    if (!string.IsNullOrEmpty(existingMovie.MovieFilePath))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory() + "wwwroot/images", existingMovie.MovieFilePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await movie.MoviePoster.CopyToAsync(stream);
                    }

                    var posterImagePath = "images/" + fullPath;
                    var posterName = movie.MoviePoster.FileName;

                    existingMovie.MovieFilePath = posterImagePath;
                    existingMovie.MovieFileName = posterName;
                    dbContext.Movies.Update(existingMovie);
                    dbContext.SaveChanges();
                    return Ok(existingMovie);

                }
            }
            return Conflict(new { error = "Failed to update movie to database" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteMovie(int id)
        {
            var existingMovie = await dbContext.Movies.SingleOrDefaultAsync(x => x.Id == id);
            if(existingMovie == null)
            {
                return NotFound();
            }
            dbContext.Movies.Remove(existingMovie);
            var rowsDeleted = dbContext.SaveChanges();
            if (rowsDeleted > 0)
                return Ok("Deleted Successfully");
            return Ok("Deleted Successfully");
        }

    }
}
