using MediatR;
using TaskForge.Domain;
using TaskForge.Persistence;

namespace Application.TaskItems
{
    public class Details
    {
        public class Query : IRequest<TaskItem>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, TaskItem>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<TaskItem> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.TaskItems.FindAsync(request.Id);
            }
        }
    }
}
