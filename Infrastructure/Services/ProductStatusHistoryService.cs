using AutoMapper;
using AutoMapper.QueryableExtensions;
using DBS_Task.Application.Common;
using DBS_Task.Application.Contracts;
using DBS_Task.Application.DTOs.ProductStatusHistory;
using DBS_Task.Application.Interfaces;
using DBS_Task.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DBS_Task.Infrastructure.Services
{
    public class ProductStatusHistoryService : IProductStatusHistoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductStatusHistoryService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<ProductStatusHistoriesResponseDto>> GetProductStatusHistoriesAsync(GetProductStatusHistoriesQueryContract queryContract)
        {
            var query = _context.ProductStatusHistories.AsNoTracking();

            if (queryContract.ProductId.HasValue)
            {
                query = query.Where(p => p.ProductId == queryContract.ProductId.Value);
            }

            if (queryContract.OldStatus.HasValue)
            {
                query = query.Where(p => p.OldStatus == queryContract.OldStatus.Value);
            }

            if (queryContract.NewStatus.HasValue)
            {
                query = query.Where(p => p.NewStatus == queryContract.NewStatus.Value);
            }

            if (queryContract.FromDate.HasValue)
            {
                var fromDateMidnight = queryContract.FromDate.Value.Date;
                query = query.Where(p => p.CreatedAt >= fromDateMidnight);
            }

            if (queryContract.ToDate.HasValue)
            {
                var toDateNextMidnight = queryContract.ToDate.Value.Date.AddDays(1);
                query = query.Where(p => p.CreatedAt < toDateNextMidnight);
            }

            var totalCount = await query.CountAsync();

            var histories = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((queryContract.PageNumber - 1) * queryContract.PageSize)
                .Take(queryContract.PageSize)
                .ProjectTo<ProductStatusHistoriesResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PaginatedResult<ProductStatusHistoriesResponseDto>(histories, queryContract.PageNumber, queryContract.PageSize, totalCount);
        }
    }
}
