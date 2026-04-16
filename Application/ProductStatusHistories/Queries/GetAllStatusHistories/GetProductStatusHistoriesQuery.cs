using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.ProductStatusHistory;
using DBS_Task.Domain.Enums;
using MediatR;

namespace DBS_Task.Application.ProductStatusHistories.Queries.GetAllStatusHistories
{
    public record GetProductStatusHistoriesQuery(int? ProductId, ProductStatus? OldStatus, ProductStatus? NewStatus, DateTime? FromDate, DateTime? ToDate, int PageNumber, int PageSize)
        : IRequest<ApiResponse<PaginatedResult<ProductStatusHistoriesResponseDto>>>
    {
    }
}
