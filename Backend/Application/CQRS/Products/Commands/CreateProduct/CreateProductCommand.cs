using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Product;
using DBS_Task.Contracts.Product;
using MediatR;

namespace DBS_Task.Application.CQRS.Products.Commands.CreateProduct
{
    public record CreateProductCommand(CreateProductContract CreateProductContract)
        : IRequest<ApiResponse<ProductResponseDto>>
    {
    }
}
