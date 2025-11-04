using MediatR;
using Microsoft.Extensions.Logging;
using TaskForge.Application.Core;
using TaskForge.Domain;
using TaskForge.Persistence;

namespace TaskForge.Application.TaskItems;

public class Details
{
    public class Query : IRequest<Result<TaskItem>>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<TaskItem>>
    {
        private readonly DataContext _context;
        private readonly ILogger<Handler> _logger;

        public Handler(DataContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<TaskItem>> Handle(Query request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            
            _logger.LogInformation("Executing query: Details TaskItem with Id: {TaskItemId}", request.Id);
            
            var taskItem = await _context.TaskItems.FindAsync(new object[] { request.Id }, cancellationToken).ConfigureAwait(false);
            
            if (taskItem == null)
            {
                _logger.LogWarning("Task item with Id: {TaskItemId} not found", request.Id);
                // Return Success with null value - HandleResult will convert this to NotFound
                return Result<TaskItem>.Success(null);
            }
            
            _logger.LogInformation("Query Details TaskItem completed successfully for Id: {TaskItemId}", request.Id);
            return Result<TaskItem>.Success(taskItem);
        }
    }
}
