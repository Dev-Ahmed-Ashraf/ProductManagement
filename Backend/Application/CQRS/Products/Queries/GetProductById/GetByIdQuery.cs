using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Product;
using MediatR;

namespace DBS_Task.Application.CQRS.Products.Queries.GetProductById
{
    public record GetByIdQuery(int Id) : IRequest<ApiResponse<ProductWHistoryResponseDto>>;
}
