using AutoMapper;
using FluentValidation;
using MediatR;
using TaskForge.Application.Core;
using TaskForge.Application.TaskItem;
using TaskForge.Domain;
using TaskForge.Persistence;

namespace Application.TaskItems
{
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
                RuleFor(x => x.TaskItem).SetValidator(new TaskItemValidator());
            }
        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var taskItem =await _context.TaskItems.FindAsync(request.TaskItem.Id);

                if (taskItem == null)
                    return null;
                _mapper.Map(request.TaskItem, taskItem);
                _context.Update(taskItem);

                var result = await _context.SaveChangesAsync() > 0;
                if (!result)
                    return Result<Unit>.Failure("Failed to update the taskItem");
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
