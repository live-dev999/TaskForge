using AutoMapper;
using MediatR;
using TaskForge.Domain;
using TaskForge.Persistence;

namespace TaskForge.Application.TaskItem
{
    public class Delete
    {
        public class Command : IRequest
        {
            public Domain.TaskItem TaskItem { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.TaskItems.FindAsync(request.TaskItem.Id);

                _mapper.Map(request.TaskItem, activity);
                _context.Update(activity);

                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
