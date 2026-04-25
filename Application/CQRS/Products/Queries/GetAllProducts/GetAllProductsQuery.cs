using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Product;
using DBS_Task.Contracts.Product;
using MediatR;

namespace DBS_Task.Application.CQRS.Products.Queries.GetAllProducts
{
    public record GetAllProductsQuery(GetProductsQueryContract ProductsQueryContract) 
        : IRequest<ApiResponse<PaginatedResult<ProductResponseDto>>>
    {
    }
}
