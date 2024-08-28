using FluentValidation;
using TodoListApi.Dto;

namespace TodoListApi.Validators
{
    public class TagDtoValidator : AbstractValidator<TagDto>
    {
        public TagDtoValidator()
        {
            // Validate Name
            RuleFor(status => status.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must be less than 50 characters.");

            // Validate Color
            RuleFor(status => status.Color)
                .NotEmpty().WithMessage("Color is required.")
                .MaximumLength(9).WithMessage("Name must be less than 9 characters.");
        }
    }
}