using FluentValidation;
using TaskForge.Domain;
namespace TaskForge.Application.TaskItems
{
    public class TaskItemValidator : AbstractValidator<TaskItem>
    {
        public TaskItemValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
            RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid task status");
            // Note: CreatedAt and UpdatedAt are set automatically, no validation needed
        }
    }
}
