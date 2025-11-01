using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskForge.Application.Core;
using TaskForge.Domain;
using TaskForge.Persistence;

namespace TaskForge.Application.TaskItems;

public class List
{
    public class Query : IRequest<Result<List<TaskItem>>> { }

    public class Handler : IRequestHandler<Query, Result<List<TaskItem>>>
    {
        private readonly DataContext _context;
        private readonly ILogger<Handler> _logger;

        public Handler(DataContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<TaskItem>>> Handle(
            Query request,
            CancellationToken cancellationToken
        )
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            
            _logger.LogInformation("Executing query: List TaskItems");
            
            var items = await _context.TaskItems.ToListAsync(cancellationToken);
            var result = Result<List<TaskItem>>.Success(items);
            
            _logger.LogInformation("Query List TaskItems completed successfully. Items count: {Count}", items.Count);
            
            return result;
        }
    }
}
