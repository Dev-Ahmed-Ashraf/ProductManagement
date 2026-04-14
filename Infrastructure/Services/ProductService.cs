using AutoMapper;
using DBS_Task.API.Responses;
using DBS_Task.Application.Common;
using DBS_Task.Application.Contracts;
using DBS_Task.Application.DTOs.Product;
using DBS_Task.Application.DTOs.ProductStatusHistory;
using DBS_Task.Application.Services;
using DBS_Task.Domain.Entities;
using DBS_Task.Domain.Enums;
using DBS_Task.Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace DBS_Task.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProductService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }


        public async Task<ProductResponseDto> CreateProductAsync(CreateProductContract createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            var ProductCreated = _mapper.Map<ProductResponseDto>(product);
            return ProductCreated;
        }

        public async Task<PaginatedResult<ProductResponseDto>> GetAllProductsAsync(GetProductsQueryContract query)
        {
            var queryable = _dbContext.Products.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                queryable = queryable.Where(p => p.Name.Contains(query.Name));
            }
            if (!string.IsNullOrWhiteSpace(query.Description))
            {
                queryable = queryable.Where(p => p.Description.Contains(query.Description));
            }
            if (query.Price.HasValue)
            {
                queryable = queryable.Where(p => p.Price == query.Price.Value);
            }
            if (query.Quantity.HasValue)
            {
                queryable = queryable.Where(p => p.Quantity == query.Quantity.Value);
            }

            var totalCount = await queryable.CountAsync();

            var products = await queryable
                .OrderByDescending(p => p.CreatedAt)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var productDtos = _mapper.Map<List<ProductResponseDto>>(products);

            return new PaginatedResult<ProductResponseDto>(productDtos, query.PageNumber, query.PageSize, totalCount);
        }

        public async Task<bool> SoftDeleteProductAsync(int productId)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            if (product == null || product.IsDeleted)
            {
                return false;
            }

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<ApiResponse<ChangeProductStatusResponseDto>> ChangeProductStatus(int productId, ProductStatus newStatus)
        {
            // Validate product exists
            var product = await _dbContext.Products.FindAsync(productId);

            if (product is null)
                return ApiResponse<ChangeProductStatusResponseDto>.FailureResponse($"Product with ID {productId} was not found.", 404);

            var statusChangeHistory = product.ChangeStatus(newStatus);

            // Validate same status
            if (statusChangeHistory is null)
                return ApiResponse<ChangeProductStatusResponseDto>.FailureResponse($"Product is already set to '{newStatus}'. No change was made.", 400);

            await _dbContext.SaveChangesAsync();

            var responseDto = new ChangeProductStatusResponseDto
            {
                ProductId = product.Id,
                OldStatus = statusChangeHistory.OldStatus,
                NewStatus = statusChangeHistory.NewStatus,
            };

            return ApiResponse<ChangeProductStatusResponseDto>.SuccessResponse(responseDto, 200, $"Product status updated to '{newStatus}' successfully.");
        }  
    }
}
