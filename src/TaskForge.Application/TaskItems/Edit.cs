using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskForge.Application.Core;
using TaskForge.Domain;
using TaskForge.Domain.Enum;
using TaskForge.Persistence;

namespace TaskForge.Application.TaskItems;

public class Edit
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
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;
        private readonly IEventService _eventService;

        public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger, IEventService eventService)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;
            _eventService = eventService;
        }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                _logger.LogInformation(
                    "Executing command: Edit TaskItem with Id: {TaskItemId}, Title: {Title}",
                    request.TaskItem.Id,
                    request.TaskItem.Title);
                
                var taskItem = await _context.TaskItems.FindAsync(new object[] { request.TaskItem.Id }, cancellationToken);

            if (taskItem == null)
            {
                _logger.LogWarning("Task item with Id: {TaskItemId} not found for update", request.TaskItem.Id);
                return Result<Unit>.Failure("Task item not found");
            }
            
            // Preserve CreatedAt before mapping
            var preservedCreatedAt = taskItem.CreatedAt;
            
            // Map properties from request to existing entity
            _mapper.Map(request.TaskItem, taskItem);
            
            // Explicitly preserve CreatedAt and update UpdatedAt after mapping
            taskItem.CreatedAt = preservedCreatedAt;
            taskItem.UpdatedAt = DateTime.UtcNow;
            
            _context.Update(taskItem);

            var result = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (!result)
            {
                _logger.LogError("Failed to update task item with Id: {TaskItemId}", request.TaskItem.Id);
                return Result<Unit>.Failure("Failed to update the taskItem");
            }
            
            _logger.LogInformation("Command Edit TaskItem completed successfully for Id: {TaskItemId}", request.TaskItem.Id);
            
            // Send event to EventProcessor (fire and forget - don't block on failure)
            _ = Task.Run(async () =>
            {
                try
                {
                    var eventDto = MapToEventDto(taskItem, "Updated");
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
