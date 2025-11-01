using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskForge.Application.Core;
using TaskForge.Domain;
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
            RuleFor(x => x.TaskItem).SetValidator(new TaskItemValidator());
        }
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
            // Auto-set ID, CreatedAt and UpdatedAt
            if (request.TaskItem.Id == Guid.Empty)
                request.TaskItem.Id = Guid.NewGuid();
                
            _logger.LogInformation(
                "Executing command: Create TaskItem with Id: {TaskItemId}, Title: {Title}",
                request.TaskItem.Id,
                request.TaskItem.Title);
            
            request.TaskItem.CreatedAt = DateTime.UtcNow;
            request.TaskItem.UpdatedAt = DateTime.UtcNow;
            
            _context.Add(request.TaskItem);

            var result = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (!result)
            {
                _logger.LogError("Failed to create task item with Id: {TaskItemId}", request.TaskItem.Id);
                return Result<Unit>.Failure("Failed to create task item");
            }
            
            _logger.LogInformation("Command Create TaskItem completed successfully for Id: {TaskItemId}", request.TaskItem.Id);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
