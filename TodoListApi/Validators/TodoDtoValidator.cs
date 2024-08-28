using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TodoListApi.Data;
using TodoListApi.Dto;

namespace TodoListApi.Validators
{
    public class TodoDtoValidator : AbstractValidator<TodoDto>
    {
        private readonly TodoListContext _context;

        public TodoDtoValidator(TodoListContext context)
        {
            _context = context;

            // Validate Name
            RuleFor(todo => todo.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(255).WithMessage("Name must be less than 255 characters.");

            // Validate PriorityId
            RuleFor(todo => todo.PriorityId)
                .MustAsync(async (priorityId, cancellation) =>
                {
                    if (priorityId.HasValue)
                    {
                        return await _context.Priorities.AnyAsync(p => p.Id == priorityId.Value);
                    }
                    return true; // null is valid
                }).WithMessage("Invalid PriorityId.");

            // Validate StatusId
            RuleFor(todo => todo.StatusId)
                .MustAsync(async (statusId, cancellation) =>
                {
                    if (statusId.HasValue)
                    {
                        return await _context.Statuses.AnyAsync(s => s.Id == statusId.Value);
                    }
                    return true; // null is valid
                }).WithMessage("Invalid StatusId.");

            // Validate Tags
            RuleForEach(todo => todo.TagIds).MustAsync(async (tagId, cancellation) =>
            {
                // Validate each Tag ID
                return await _context.Tags.AnyAsync(t => t.Id == tagId);
            }).WithMessage("One or more Tag IDs are invalid.");
        }
    }
}