using FluentValidation;
using MediatR;
using TaskForge.Application.Core;
using TaskForge.Application.TaskItem;
using TaskForge.Domain;
using TaskForge.Persistence;

namespace Application.TaskItems
{
    public class Create
    {
        public class Command : IRequest<Result<Unit>>
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
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(
                Command request,
                CancellationToken cancellationToken
            )
            {
                _context.Add(request.TaskItem);

                var result = await _context.SaveChangesAsync() > 0;
                if (!result)
                    return Result<Unit>.Failure("Failed to create task item");
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
