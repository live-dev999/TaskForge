using AutoMapper;
using MediatR;
using TaskForge.Application.Core;
using TaskForge.Persistence;

namespace TaskForge.Application.TaskItem
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
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

            public async Task<Result<Unit>> Handle(
                Command request,
                CancellationToken cancellationToken
            )
            {
                var taskItem = await _context.TaskItems.FindAsync(request.Id);

                if (taskItem == null)
                    return null;

                _context.Remove(taskItem);

                var result = await _context.SaveChangesAsync() > 0;
                if (!result)
                    return Result<Unit>.Failure("Failed to delete the taskItem");
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
