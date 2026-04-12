using DBS_Task.Application.Common;
using DBS_Task.Application.Contracts;
using DBS_Task.Application.DTOs.Product;
using DBS_Task.Domain.Entities;

namespace DBS_Task.Application.Services
{
    public interface IProductService
    {
        Task<ProductResponseDto> CreateProductAsync(CreateProductContract createProductDto);
        Task<PaginatedResult<ProductResponseDto>> GetAllProductsAsync(GetProductsQueryContract query);
        Task<bool> SoftDeleteProductAsync(int productId);
    }
}
