using AutoMapper;
using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Product;
using DBS_Task.Infrastructure.Data.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DBS_Task.Application.Products.Queries.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, ApiResponse<PaginatedResult<ProductResponseDto>>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetAllProductsQueryHandler(ApplicationDbContext dbcontext, IMapper mapper)
        {
            _dbContext = dbcontext;
            _mapper = mapper;
        }
        public async Task<ApiResponse<PaginatedResult<ProductResponseDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var queryable = _dbContext.Products.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.ProductsQueryContract.Name))
            {
                queryable = queryable.Where(p => p.Name.Contains(request.ProductsQueryContract.Name));
            }
            if (!string.IsNullOrWhiteSpace(request.ProductsQueryContract.Description))
            {
                queryable = queryable.Where(p => p.Description != null && p.Description.Contains(request.ProductsQueryContract.Description));
            }
            if (request.ProductsQueryContract.Price.HasValue)
            {
                queryable = queryable.Where(p => p.Price == request.ProductsQueryContract.Price.Value);
            }
            if (request.ProductsQueryContract.Quantity.HasValue)
            {
                queryable = queryable.Where(p => p.Quantity == request.ProductsQueryContract.Quantity.Value);
            }

            var totalCount = await queryable.CountAsync(cancellationToken);

            var products = await queryable
                .OrderByDescending(p => p.CreatedAt)
                .Skip((request.ProductsQueryContract.PageNumber - 1) * request.ProductsQueryContract.PageSize)
                .Take(request.ProductsQueryContract.PageSize)
                .ToListAsync();

            var productDtos = _mapper.Map<List<ProductResponseDto>>(products);

            return ApiResponse<PaginatedResult<ProductResponseDto>>
                    .SuccessResponse(
                        new PaginatedResult<ProductResponseDto>(
                            productDtos,
                            request.ProductsQueryContract.PageNumber,
                            request.ProductsQueryContract.PageSize,
                            totalCount
                    ),
                        200,
                        "Products retrieved successfully"
                        );

        }
    }
}
