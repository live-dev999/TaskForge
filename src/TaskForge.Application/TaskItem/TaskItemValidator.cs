using FluentValidation;
namespace TaskForge.Application.TaskItem
{
    public class TaskItemValidator : AbstractValidator<Domain.TaskItem>
    {
        public TaskItemValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.CreatedAt).NotEmpty();
            RuleFor(x => x.UpdatedAt).NotEmpty();
            RuleFor(x => x.Status).NotEmpty();
        }
    }
}
