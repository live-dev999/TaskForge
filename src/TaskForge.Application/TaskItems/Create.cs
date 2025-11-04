using AutoMapper;
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
    public class Command : IRequest<Result<TaskItemDto>>
    {
        public TaskItemDto TaskItem { get; set; }
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
    public class Handler : IRequestHandler<Command, Result<TaskItemDto>>
    {
        private readonly DataContext _context;
        private readonly ILogger<Handler> _logger;
        private readonly IEventService _eventService;
        private readonly IMessageProducer _messageProducer;
        private readonly IMapper _mapper;

        public Handler(DataContext context, ILogger<Handler> logger, IEventService eventService, IMessageProducer messageProducer, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _eventService = eventService;
            _messageProducer = messageProducer;
            _mapper = mapper;
        }

        public async Task<Result<TaskItemDto>> Handle(
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
            
            // Map TaskItemDto to TaskItem entity for database persistence
            var taskItemEntity = _mapper.Map<TaskItem>(request.TaskItem);
            _context.Add(taskItemEntity);

            var result = await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0;
            if (!result)
            {
                _logger.LogError("Failed to create task item with Id: {TaskItemId}", request.TaskItem.Id);
                return Result<TaskItemDto>.Failure("Failed to create task item");
            }
            
            _logger.LogInformation("Command Create TaskItem completed successfully for Id: {TaskItemId}", request.TaskItem.Id);
            
            // Map back to DTO for response
            var taskItemDto = _mapper.Map<TaskItemDto>(taskItemEntity);
            
            // Send events (synchronous HTTP and asynchronous RabbitMQ) - fire and forget - don't block on failure
            // Use separate CancellationTokenSource with timeout to ensure background task completes even if HTTP request is cancelled
            _ = Task.Run(async () =>
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)); // 30 second timeout for background task
                try
                {
                    var eventDto = MapToEventDto(taskItemDto, "Created");
                    
                    // Send to EventProcessor (synchronous HTTP)
                    await _eventService.SendEventAsync(eventDto, cts.Token).ConfigureAwait(false);
                    
                    // Publish to RabbitMQ (asynchronous)
                    await _messageProducer.PublishEventAsync(eventDto, cts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Background task for sending events was cancelled: TaskId={TaskId}", taskItemDto.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in background task for sending events: TaskId={TaskId}", taskItemDto.Id);
                }
            });
            
            return Result<TaskItemDto>.Success(taskItemDto);
        }

        private static TaskChangeEventDto MapToEventDto(TaskItemDto taskItem, string eventType)
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
