namespace MovieApplicationApi.Validations;
using FluentValidation;
using WebApplication1.Models.Entities;

public class GenreValidator : AbstractValidator<Genre>     
{
    public GenreValidator()
    {
        RuleFor(genre => genre.GenreName)
            .NotEmpty().WithMessage("Genre name is required.")
            .Length(2, 50).WithMessage("Genre name must be between 2 and 50 characters long.")
            .Matches(@"^[a-zA-Z\s\-]+$").WithMessage("Genre Name should only contain alphabets and hyphens");
    }
}
