using MediatR;
using Microsoft.Extensions.Logging;
using TaskForge.Application.Core;
using TaskForge.Persistence;

namespace TaskForge.Application.TaskItems;

public class Delete
{
    public class Command : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _context;
        private readonly ILogger<Handler> _logger;

        public Handler(DataContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(
            Command request,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Executing command: Delete TaskItem with Id: {TaskItemId}", request.Id);
            
            var taskItem = await _context.TaskItems.FindAsync(new object[] { request.Id }, cancellationToken);

            if (taskItem == null)
            {
                _logger.LogWarning("Task item with Id: {TaskItemId} not found for deletion", request.Id);
                return Result<Unit>.Failure("Task item not found");
            }

            _context.Remove(taskItem);

            var result = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (!result)
            {
                _logger.LogError("Failed to delete task item with Id: {TaskItemId}", request.Id);
                return Result<Unit>.Failure("Failed to delete the taskItem");
            }
            
            _logger.LogInformation("Command Delete TaskItem completed successfully for Id: {TaskItemId}", request.Id);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
