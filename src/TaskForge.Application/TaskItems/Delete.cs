using MediatR;
using Microsoft.Extensions.Logging;
using TaskForge.Application.Core;
using TaskForge.Domain.Enum;
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
        private readonly IEventService _eventService;
        private readonly IMessageProducer _messageProducer;

        public Handler(DataContext context, ILogger<Handler> logger, IEventService eventService, IMessageProducer messageProducer)
        {
            _context = context;
            _logger = logger;
            _eventService = eventService;
            _messageProducer = messageProducer;
        }

        public async Task<Result<Unit>> Handle(
            Command request,
            CancellationToken cancellationToken
        )
        {
            _logger.LogInformation("Executing command: Delete TaskItem with Id: {TaskItemId}", request.Id);
            
            var taskItem = await _context.TaskItems.FindAsync(new object[] { request.Id }, cancellationToken).ConfigureAwait(false);

            if (taskItem == null)
            {
                _logger.LogWarning("Task item with Id: {TaskItemId} not found for deletion", request.Id);
                return Result<Unit>.Failure("Task item not found");
            }

            // Store task data before deletion for event
            var deletedTaskData = new
            {
                taskItem.Id,
                taskItem.Title,
                taskItem.Description,
                taskItem.Status,
                taskItem.CreatedAt,
                taskItem.UpdatedAt
            };

            _context.Remove(taskItem);

            var result = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0;
            if (!result)
            {
                _logger.LogError("Failed to delete task item with Id: {TaskItemId}", request.Id);
                return Result<Unit>.Failure("Failed to delete the taskItem");
            }
            
            _logger.LogInformation("Command Delete TaskItem completed successfully for Id: {TaskItemId}", request.Id);
            
            // Send events (synchronous HTTP and asynchronous RabbitMQ) - fire and forget - don't block on failure
            // Use separate CancellationTokenSource with timeout to ensure background task completes even if HTTP request is cancelled
            _ = Task.Run(async () =>
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)); // 30 second timeout for background task
                try
                {
                    var eventDto = new TaskChangeEventDto
                    {
                        TaskId = deletedTaskData.Id,
                        EventType = "Deleted",
                        Title = deletedTaskData.Title,
                        Description = deletedTaskData.Description,
                        Status = deletedTaskData.Status.ToString(),
                        EventTimestamp = DateTime.UtcNow,
                        CreatedAt = deletedTaskData.CreatedAt,
                        UpdatedAt = deletedTaskData.UpdatedAt
                    };
                    
                    // Send to EventProcessor (synchronous HTTP)
                    await _eventService.SendEventAsync(eventDto, cts.Token).ConfigureAwait(false);
                    
                    // Publish to RabbitMQ (asynchronous)
                    await _messageProducer.PublishEventAsync(eventDto, cts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Background task for sending events was cancelled: TaskId={TaskId}", request.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in background task for sending events: TaskId={TaskId}", request.Id);
                }
            });
            
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
