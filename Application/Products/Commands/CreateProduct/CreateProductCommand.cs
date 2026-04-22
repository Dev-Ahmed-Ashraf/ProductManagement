using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Product;
using DBS_Task.Contracts;
using MediatR;

namespace DBS_Task.Application.Products.Commands.CreateProduct
{
    public record CreateProductCommand(CreateProductContract CreateProductContract)
        : IRequest<ApiResponse<ProductResponseDto>>
    {
    }
}
