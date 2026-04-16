using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Product;
using MediatR;

namespace DBS_Task.Application.Products.Commands.CreateProduct
{
    public record CreateProductCommand(string name, string? description, decimal price, int quantity)
        : IRequest<ApiResponse<ProductResponseDto>>
    {
    }
}
