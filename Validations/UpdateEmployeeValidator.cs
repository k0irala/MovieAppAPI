using FluentValidation;
using MovieApplicationApi.Models;

namespace MovieApplicationApi.Validations
{
    public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeDTO>
    {
        public UpdateEmployeeValidator() {
            RuleFor(r => r.Address)
                .NotEmpty().WithMessage("Employee Address is Required");
            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Employee Email is Required")
                .Matches(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$").WithMessage("Enter a valid email");
            RuleFor(r => r.Salary)
                .NotEmpty().WithMessage("Employee Salary is Required");
            RuleFor(r => r.Name)
                .NotEmpty().WithMessage("Employee Name is Required")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Name should not contain any letters!"); 
        }
    }
}
