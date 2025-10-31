using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskForge.Application.Core;
using TaskForge.Domain;
using TaskForge.Persistence;

namespace Application.TaskItems
{
    public class List
    {
        public class Query : IRequest<Result<List<TaskItem>>> { }

        public class Handler : IRequestHandler<Query, Result<List<TaskItem>>>
        {
            private readonly DataContext _context;
            private readonly ILogger _logger;

            public Handler(DataContext context, ILogger<Handler> logger)
            {
                _logger = logger;
                _context = context;
            }

            public async Task<Result<List<TaskItem>>> Handle(
                Query request,
                CancellationToken cancellationToken
            )
            {
                return Result<List<TaskItem>>.Success(await _context.TaskItems.ToListAsync());
            }
        }
    }
}
