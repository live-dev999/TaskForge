using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskForge.Persistence;
using TaskForge.Domain;

namespace Application.TaskItems
{
    public class List
    {
        public class Query : IRequest<List<TaskItem>> { }

        public class Handler : IRequestHandler<Query, List<TaskItem>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<List<TaskItem>> Handle(
                Query request,
                CancellationToken cancellationToken
            )
            {
                return await _context.TaskItems.ToListAsync();
            }
        }
    }
}
