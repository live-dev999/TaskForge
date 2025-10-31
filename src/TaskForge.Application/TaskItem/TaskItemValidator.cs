using FluentValidation;
namespace TaskForge.Application.TaskItem
{
    public class TaskItemValidator : AbstractValidator<Domain.TaskItem>
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
