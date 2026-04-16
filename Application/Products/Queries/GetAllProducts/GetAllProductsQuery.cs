using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Product;
using DBS_Task.Contracts;
using MediatR;

namespace DBS_Task.Application.Products.Queries.GetAllProducts
{
    public record GetAllProductsQuery(string? Name, string? Description, decimal? Price, int? Quantity, int PageNumber, int PageSize) 
        : IRequest<ApiResponse<PaginatedResult<ProductResponseDto>>>
    {
    }
}
