using AutoMapper;
using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Product;
using DBS_Task.Infrastructure.Data.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DBS_Task.Application.CQRS.Products.Queries.GetAllProducts
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
        public async Task<ApiResponse<PaginatedResult<ProductResponseDto>>> Handle(GetAllProductsQuery query, CancellationToken cancellationToken)
        {
            var request = query.ProductsQueryContract;
            var queryable = _dbContext.Products.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                queryable = queryable.Where(p => p.Name.Contains(request.Name));
            }
            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                queryable = queryable.Where(p => p.Description != null && p.Description.Contains(request.Description));
            }
            if (request.Price.HasValue)
            {
                queryable = queryable.Where(p => p.Price == request.Price.Value);
            }
            if (request.Quantity.HasValue)
            {
                queryable = queryable.Where(p => p.Quantity == request.Quantity.Value);
            }

            var totalCount = await queryable.CountAsync(cancellationToken);

            var products = await queryable
                .OrderByDescending(p => p.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var productDtos = _mapper.Map<List<ProductResponseDto>>(products);

            return ApiResponse<PaginatedResult<ProductResponseDto>>
                    .SuccessResponse(
                        new PaginatedResult<ProductResponseDto>(
                            productDtos,
                            request.PageNumber,
                            request.PageSize,
                            totalCount
                    ),
                        200,
                        "Products retrieved successfully"
                        );

        }
    }
}
