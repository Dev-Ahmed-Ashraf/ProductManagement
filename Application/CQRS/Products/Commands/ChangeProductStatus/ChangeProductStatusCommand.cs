using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.ProductStatusHistory;
using DBS_Task.Domain.Enums;
using MediatR;

namespace DBS_Task.Application.CQRS.Products.Commands.ChangeProductStatus
{
    public record ChangeProductStatusCommand(
        int ProductId,
        ProductStatus NewStatus
    ) : IRequest<ApiResponse<ChangeProductStatusResponseDto>>;
}
