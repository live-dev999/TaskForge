using FluentValidation;
using TaskForge.Domain;

namespace TaskForge.Application.TaskItems;

public class TaskItemValidator : AbstractValidator<TaskItem>
{
    public TaskItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(500).WithMessage("Title must not exceed 500 characters");
        
        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");
        
        RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid task status");
        // Note: CreatedAt and UpdatedAt are set automatically, no validation needed
        // Note: Description is optional (can be null/empty) as per database configuration
    }
}
