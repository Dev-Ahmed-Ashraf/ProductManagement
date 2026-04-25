using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Statistics;
using DBS_Task.Domain.Enums;
using DBS_Task.Infrastructure.Data.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DBS_Task.Application.Statistics.Queries
{
    public class GetStatisticsQueryHandler : IRequestHandler<GetStatisticsQuery, ApiResponse<StatisticsResponseDto>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetStatisticsQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ApiResponse<StatisticsResponseDto>> Handle(
            GetStatisticsQuery request,
            CancellationToken cancellationToken)
        {

            // Fetch product status counts
            var productStatusCounts = await _dbContext.Products
                .AsNoTracking()
                .GroupBy(p => p.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(
                    x => x.Status,
                    x => x.Count,
                    cancellationToken);

            // Fetch user activity counts
            var userActivityCounts = await _dbContext.Users
                .AsNoTracking()
                .GroupBy(u => u.IsActive)
                .Select(g => new
                {
                    IsActive = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(
                    x => x.IsActive,
                    x => x.Count,
                    cancellationToken);

            // Fetch total status changes count
            var statusChangesCount =
                await _dbContext.ProductStatusHistories
                    .AsNoTracking()
                    .CountAsync(cancellationToken);

            // Construct the response DTO
            var response = new StatisticsResponseDto
            {
                Products = new ProductStatisticsDto
                {
                    Total =
                        productStatusCounts.Values.Sum(),

                    Available =
                        productStatusCounts.GetValueOrDefault(
                            ProductStatus.Available),

                    OutOfStock =
                        productStatusCounts.GetValueOrDefault(
                            ProductStatus.OutOfStock),

                    Discontinued =
                        productStatusCounts.GetValueOrDefault(
                            ProductStatus.Discontinued),

                    PreOrder = 
                        productStatusCounts.GetValueOrDefault(
                            ProductStatus.PreOrder),

                    BackOrder = 
                        productStatusCounts.GetValueOrDefault(
                            ProductStatus.BackOrder),

                    Draft = 
                        productStatusCounts.GetValueOrDefault(ProductStatus.Draft)
                },

                Users = new UserStatisticsDto
                {
                    Total =
                        userActivityCounts.Values.Sum(),

                    Active =
                        userActivityCounts.GetValueOrDefault(true),

                    Inactive =
                        userActivityCounts.GetValueOrDefault(false)
                },

                StatusChanges = new StatusChangeStatisticsDto
                {
                    Total = statusChangesCount
                }
            };


            return ApiResponse<StatisticsResponseDto>
                .SuccessResponse(
                    response,
                    200,
                    "Statistics retrieved successfully.");
        }
    }
}
