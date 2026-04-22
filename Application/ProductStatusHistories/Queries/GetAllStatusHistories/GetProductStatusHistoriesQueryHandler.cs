using AutoMapper;
using AutoMapper.QueryableExtensions;
using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.ProductStatusHistory;
using DBS_Task.Infrastructure.Data.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DBS_Task.Application.ProductStatusHistories.Queries.GetAllStatusHistories
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

        public async Task<ApiResponse<PaginatedResult<ProductStatusHistoriesResponseDto>>> Handle(GetProductStatusHistoriesQuery request, CancellationToken cancellationToken)
        {
            var queryable = _dbContext.ProductStatusHistories.AsNoTracking();

            if (request.HistoriesQueryContract.ProductId.HasValue)
            {
                queryable = queryable.Where(p => p.ProductId == request.HistoriesQueryContract.ProductId.Value);
            }

            if (request.HistoriesQueryContract.OldStatus.HasValue)
            {
                queryable = queryable.Where(p => p.OldStatus == request.HistoriesQueryContract.OldStatus.Value);
            }

            if (request.HistoriesQueryContract.NewStatus.HasValue)
            {
                queryable = queryable.Where(p => p.NewStatus == request.HistoriesQueryContract.NewStatus.Value);
            }

            if (request.HistoriesQueryContract.FromDate.HasValue)
            {
                var fromDateMidnight = request.HistoriesQueryContract.FromDate.Value.Date;
                queryable = queryable.Where(p => p.CreatedAt >= fromDateMidnight);
            }

            if (request.HistoriesQueryContract.ToDate.HasValue)
            {
                var toDateNextMidnight = request.HistoriesQueryContract.ToDate.Value.Date.AddDays(1);
                queryable = queryable.Where(p => p.CreatedAt < toDateNextMidnight);
            }

            var totalCount = await queryable.CountAsync(cancellationToken);

            var histories = await queryable
                .OrderByDescending(p => p.CreatedAt)
                .Skip((request.HistoriesQueryContract.PageNumber - 1) * request.HistoriesQueryContract.PageSize)
                .Take(request.HistoriesQueryContract.PageSize)
                .ProjectTo<ProductStatusHistoriesResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return ApiResponse<PaginatedResult<ProductStatusHistoriesResponseDto>>.SuccessResponse(
                new PaginatedResult<ProductStatusHistoriesResponseDto>(histories, request.HistoriesQueryContract.PageNumber, request.HistoriesQueryContract.PageSize, totalCount), 200, "Product status histories retrieved successfully"
            );
        }
    }
}
