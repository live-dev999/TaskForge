using MediatR;
using TaskForge.Domain;
using TaskForge.Persistence;

namespace Application.TaskItems
{
    public class Create
    {
        public class Command : IRequest
        {
            public TaskItem TaskItem { get; set; }
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
                _context.Add(request.TaskItem);

                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
