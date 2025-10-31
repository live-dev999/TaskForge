using MediatR;
using TaskForge.Application.Core;
using TaskForge.Domain;
using TaskForge.Persistence;

namespace Application.TaskItems
{
    public class Details
    {
        public class Query : IRequest<Result<TaskItem>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<TaskItem>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<TaskItem>> Handle(Query request, CancellationToken cancellationToken)
            {
                var taskItem = await _context.TaskItems.FindAsync(request.Id);
                return Result<TaskItem>.Success(taskItem);
            }
        }
    }
}
