using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.ProductStatusHistory;
using DBS_Task.Contracts.ProductHistory;
using DBS_Task.Domain.Enums;
using MediatR;

namespace DBS_Task.Application.CQRS.ProductStatusHistories.Queries.GetAllStatusHistories
{
    public record GetProductStatusHistoriesQuery(GetProductStatusHistoriesQueryContract HistoriesQueryContract)
        : IRequest<ApiResponse<PaginatedResult<ProductStatusHistoriesResponseDto>>>
    {
    }
}
