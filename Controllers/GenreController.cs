using System.Data;
using Dapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models.Entities;

namespace WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GenreController(ApplicationDbContext dbContext, IDapperRepository repository,IValidator<Genre> validator) : ControllerBase
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
        ValidationResult result = validator.Validate(genre);
        if (!result.IsValid)
        {
            return BadRequest(result.Errors.Select(s => new { field = s.PropertyName, error = s.ErrorMessage }));
        }
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
        //var existingGenre = dbContext.Genres.SingleOrDefault(x => x.Id == id);
        //if (existingGenre == null)
        //{
        //    return NotFound();
        //}
        //var duplicateCheck = dbContext.Genres.SingleOrDefault(x => x.GenreName == genre.GenreName && x.Id != genre.Id);
        //if (duplicateCheck == null)
        //{
        //    //existingGenre.GenreName = genre.GenreName;
        //    //dbContext.Genres.Update(existingGenre);
        //    //dbContext.SaveChanges();
        if(id == 0 || genre.GenreName == null)
        {
            return BadRequest("Genre ID or Genre Name is required");
        }
        DynamicParameters parameters = new();
        parameters.Add("@genreId", id);
        parameters.Add("@genreName", genre.GenreName);
        parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
        repository.Query<Genre>("UpdateGenreData", parameters);
        int result = parameters.Get<int>("@Result");
        if(result == -1)
        {
            return NotFound("The requested Genre is not found");
        }
        if(result == 0)
        {
            return BadRequest("The genre already exists");
        }
        return Ok("Genre Updated Successfully");
        //}
        //return Conflict(new { error = "Genre already exists" });
    }
    [HttpDelete("{id}")]
    public IActionResult deleteGenre(int id)
    {
        DynamicParameters parameters = new();
        parameters.Add("@genreId", id);
        parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
        repository.Execute("DeleteGenreData", parameters);

        var result = parameters.Get<int>("@Result");
        if(result == -1)
        {
            return NotFound("The Genre is not found");
        }

        //var existingGenre = dbContext.Genres.SingleOrDefault(y => y.Id == id);
        //if (existingGenre == null)
        //{
        //    return NotFound();
        //}
        //dbContext.Genres.Remove(existingGenre);
        //dbContext.SaveChanges();
        return Ok("The genre has been deleted successfully");
    }
}
