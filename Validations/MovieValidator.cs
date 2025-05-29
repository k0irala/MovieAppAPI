using FluentValidation;
using Microsoft.EntityFrameworkCore.Storage.Json;
using WebApplication1.Models.Entities;

namespace MovieApplicationApi.Validations;

public class MovieValidator : AbstractValidator<Movie>
{
    public MovieValidator()
    {
        RuleFor(movie => movie.Title)
            .NotEmpty().WithMessage("Movie Title is Required");
        RuleFor(movie => movie.ReleaseDate)
            .NotEmpty().WithMessage("Movie Release Date is Required");
        RuleFor(movie => movie.Rating)
            .NotEmpty().WithMessage("Movie Rating is Required");
        RuleFor(movie => movie.MoviePoster)
            .NotEmpty().WithMessage("Movie Poster is Required");
    }
}
