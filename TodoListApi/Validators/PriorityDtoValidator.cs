using FluentValidation;
using TodoListApi.Dto;

namespace TodoListApi.Validators
{
    public class PriorityDtoValidator : AbstractValidator<PriorityDto>
    {
        public PriorityDtoValidator()
        {
            // Validate Name
            RuleFor(priority => priority.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must be less than 50 characters.");

            // Validate Color
            RuleFor(priority => priority.Color)
                .NotEmpty().WithMessage("Color is required.")
                .MaximumLength(9).WithMessage("Name must be less than 9 characters.");
        }
    }
}