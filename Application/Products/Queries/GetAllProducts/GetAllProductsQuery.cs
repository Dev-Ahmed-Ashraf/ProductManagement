using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Product;
using DBS_Task.Contracts;
using MediatR;

namespace DBS_Task.Application.Products.Queries.GetAllProducts
{
    public record GetAllProductsQuery(GetProductsQueryContract ProductsQueryContract) 
        : IRequest<ApiResponse<PaginatedResult<ProductResponseDto>>>
    {
    }
}
