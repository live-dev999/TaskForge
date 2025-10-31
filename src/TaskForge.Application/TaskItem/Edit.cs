using FluentValidation;
using MediatR;
using TaskForge.Application.TaskItem;
using TaskForge.Domain;
using TaskForge.Persistence;

namespace Application.TaskItems
{
    public class Edit
    {
        public class Command : IRequest
        {
            public TaskItem TaskItem { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.TaskItem).SetValidator(new TaskItemValidator());
            }
        }
        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var TaskItem =await _context.TaskItems.FindAsync(request.TaskItem.Id);

                TaskItem.Title = request.TaskItem.Title ?? TaskItem.Title;

                _context.Update(TaskItem);

                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
