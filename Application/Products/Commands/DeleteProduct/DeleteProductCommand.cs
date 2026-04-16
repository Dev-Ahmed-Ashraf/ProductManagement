using DBS_Task.Application.Common.Results;
using MediatR;

namespace DBS_Task.Application.Products.Commands.DeleteProduct
{
    public record DeleteProductCommand(int Id) : IRequest<ApiResponse<bool>>
    {
    }
}
