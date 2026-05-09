using AutoMapper;
using AutoMapper.QueryableExtensions;
using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.ProductStatusHistory;
using DBS_Task.Infrastructure.Data.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DBS_Task.Application.CQRS.ProductStatusHistories.Queries.GetAllStatusHistories
{
    public class GetProductStatusHistoriesQueryHandler : 
        IRequestHandler<GetProductStatusHistoriesQuery, ApiResponse<PaginatedResult<ProductStatusHistoriesResponseDto>>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetProductStatusHistoriesQueryHandler(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext; 
            _mapper = mapper;
        }

        public async Task<ApiResponse<PaginatedResult<ProductStatusHistoriesResponseDto>>> Handle(GetProductStatusHistoriesQuery query, CancellationToken cancellationToken)
        {
            var request = query.HistoriesQueryContract;
            var queryable = _dbContext.ProductStatusHistories.AsNoTracking();

            if (request.ProductId.HasValue)
            {
                queryable = queryable.Where(p => p.ProductId == request.ProductId.Value);
            }

            if (request.OldStatus.HasValue)
            {
                queryable = queryable.Where(p => p.OldStatus == request.OldStatus.Value);
            }

            if (request.NewStatus.HasValue)
            {
                queryable = queryable.Where(p => p.NewStatus == request.NewStatus.Value);
            }

            if (request.FromDate.HasValue)
            {
                var fromDateMidnight = request.FromDate.Value.Date;
                queryable = queryable.Where(p => p.CreatedAt >= fromDateMidnight);
            }

            if (request.ToDate.HasValue)
            {
                var toDateNextMidnight = request.ToDate.Value.Date.AddDays(1);
                queryable = queryable.Where(p => p.CreatedAt < toDateNextMidnight);
            }

            var totalCount = await queryable.CountAsync(cancellationToken);

            var histories = await queryable
                .OrderByDescending(p => p.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ProjectTo<ProductStatusHistoriesResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return ApiResponse<PaginatedResult<ProductStatusHistoriesResponseDto>>.SuccessResponse(
                new PaginatedResult<ProductStatusHistoriesResponseDto>(histories, request.PageNumber, request.PageSize, totalCount), 200, "Product status histories retrieved successfully"
            );
        }
    }
}
