using FluentValidation;
using WebApplication1.Models;
using WebApplication1.Models.Entities;

namespace MovieApplicationApi.Validations
{
    public class EmployeeValidator : AbstractValidator<AddEmployeeDTO>
    {
        public EmployeeValidator()
        {
            RuleFor(r => r.Address)
                .NotEmpty().WithMessage("Employee Addresss is Required");
            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Employee Addresss is Required");
            RuleFor(r => r.Salary)
                .NotEmpty().WithMessage("Employee Addresss is Required");
            RuleFor(r => r.Name)
                .NotEmpty().WithMessage("Employee Addresss is Required");
        }
    }
}
