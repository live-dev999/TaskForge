using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskForge.Application.Core;
using TaskForge.Domain;
using TaskForge.Persistence;

namespace TaskForge.Application.TaskItems;

public class List
{
    public class Query : IRequest<Result<PagedList<TaskItemDto>>> { 
        public PagingParams Params { get; set; } 
    }

    public class Handler : IRequestHandler<Query, Result<PagedList<TaskItemDto>>>
    {
        private readonly DataContext _context;
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;

        public Handler(DataContext context, ILogger<Handler> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<TaskItemDto>>> Handle(
            Query request,
            CancellationToken cancellationToken
        )
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Executing query: List TaskItems");

            // Use AsNoTracking for read-only query to improve performance
            var query = _context
                    .TaskItems.AsNoTracking()
                    .OrderByDescending(_ => _.CreatedAt)
                    .ProjectTo<TaskItemDto>(_mapper.ConfigurationProvider)
                    .AsQueryable();

            _logger.LogInformation("Query List TaskItems completed successfully. Items count: {Count}", query.Count());

            return Result<PagedList<TaskItemDto>>.Success(
                    await PagedList<TaskItemDto>.CreateAsync(
                        query,
                        request.Params.PageNumber,
                        request.Params.PageSize
              )
            );
        }
    }
}
