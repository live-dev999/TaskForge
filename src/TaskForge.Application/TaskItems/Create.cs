using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskForge.Application.Core;
using TaskForge.Domain;
using TaskForge.Domain.Enum;
using TaskForge.Persistence;

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
        private readonly DataContext _context;
        private readonly ILogger<Handler> _logger;
        private readonly IEventService _eventService;

        public Handler(DataContext context, ILogger<Handler> logger, IEventService eventService)
        {
            _context = context;
            _logger = logger;
            _eventService = eventService;
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
            
            // Send event to EventProcessor (fire and forget - don't block on failure)
            _ = Task.Run(async () =>
            {
                try
                {
                    var eventDto = MapToEventDto(request.TaskItem, "Created");
                    await _eventService.SendEventAsync(eventDto, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in background task for sending event: TaskId={TaskId}", request.TaskItem.Id);
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
