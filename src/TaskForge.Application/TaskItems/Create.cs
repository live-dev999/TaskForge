using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskForge.Application.Core;
using TaskForge.Domain;

namespace TaskForge.Application.TaskItems;

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
            RuleFor(x => x.TaskItem)
                .NotNull().WithMessage("TaskItem is required")
                .SetValidator(new TaskItemValidator());
        }
    }
    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly IDataContext _context;
        private readonly ILogger<Handler> _logger;
        private readonly IEventService _eventService;
        private readonly IMessageProducer _messageProducer;

        public Handler(IDataContext context, ILogger<Handler> logger, IEventService eventService, IMessageProducer messageProducer)
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
            // Auto-set ID, CreatedAt and UpdatedAt
            if (request.TaskItem.Id == Guid.Empty)
                request.TaskItem.Id = Guid.NewGuid();
                
            _logger.LogInformation(
                "Executing command: Create TaskItem with Id: {TaskItemId}, Title: {Title}",
                request.TaskItem.Id,
                request.TaskItem.Title);
            
            var now = DateTime.UtcNow;
            request.TaskItem.CreatedAt = now;
            request.TaskItem.UpdatedAt = now;
            
            _context.Add(request.TaskItem);

            var result = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (!result)
            {
                _logger.LogError("Failed to create task item with Id: {TaskItemId}", request.TaskItem.Id);
                return Result<Unit>.Failure("Failed to create task item");
            }
            
            _logger.LogInformation("Command Create TaskItem completed successfully for Id: {TaskItemId}", request.TaskItem.Id);
            
            // Send events (synchronous HTTP and asynchronous RabbitMQ) - fire and forget - don't block on failure
            _ = Task.Run(async () =>
            {
                try
                {
                    var eventDto = MapToEventDto(request.TaskItem, "Created");
                    
                    // Send to EventProcessor (synchronous HTTP)
                    await _eventService.SendEventAsync(eventDto, cancellationToken);
                    
                    // Publish to RabbitMQ (asynchronous)
                    await _messageProducer.PublishEventAsync(eventDto, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in background task for sending events: TaskId={TaskId}", request.TaskItem.Id);
                }
            }, cancellationToken);
            
            return Result<Unit>.Success(Unit.Value);
        }

        private static TaskChangeEventDto MapToEventDto(TaskItem taskItem, string eventType)
        {
            return new TaskChangeEventDto
            {
                TaskId = taskItem.Id,
                EventType = eventType,
                Title = taskItem.Title,
                Description = taskItem.Description,
                Status = taskItem.Status.ToString(),
                EventTimestamp = DateTime.UtcNow,
                CreatedAt = taskItem.CreatedAt,
                UpdatedAt = taskItem.UpdatedAt
            };
        }
    }
}
