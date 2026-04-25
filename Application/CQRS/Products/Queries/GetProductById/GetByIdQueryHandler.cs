using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Product;
using DBS_Task.Application.DTOs.ProductStatusHistory;
using DBS_Task.Infrastructure.Data.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DBS_Task.Application.CQRS.Products.Queries.GetProductById
{
    public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, ApiResponse<ProductWHistoryResponseDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetByIdQueryHandler(ApplicationDbContext context)
        { 
            _context = context;
        }

        public async Task<ApiResponse<ProductWHistoryResponseDto>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.Products
           .AsNoTracking()
           .Where(p => p.Id == request.Id)
           .Select(p => new ProductWHistoryResponseDto
                   {
                       Id = p.Id,
                       Name = p.Name,
                       Description = p.Description,
                       Price = p.Price,
                       Quantity = p.Quantity,
                       CreatedAt = p.CreatedAt,
                       CreatedBy = p.CreatedBy,

                       History = _context.ProductStatusHistories
                           .Where(h => h.ProductId == p.Id)
                           .OrderByDescending(h => h.CreatedAt)
                           .Take(5)
                           .Select(h => new ProductStatusHistoriesResponseDto
                           {
                               Id = h.Id,
                               OldStatus = h.OldStatus,
                               NewStatus = h.NewStatus,
                               CreatedBy = h.CreatedBy,
                               CreatedAt = h.CreatedAt
                           })
                           .ToList()
                   })
                   .FirstOrDefaultAsync(cancellationToken);

            if (result == null)
            {
                return ApiResponse<ProductWHistoryResponseDto>.FailureResponse("Product not found", 404);
            }


            return ApiResponse<ProductWHistoryResponseDto>.SuccessResponse(result, message: "Product retrieved successfully");
        }
    }
}
