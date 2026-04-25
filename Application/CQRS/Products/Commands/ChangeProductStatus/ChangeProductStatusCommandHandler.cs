using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.ProductStatusHistory;
using DBS_Task.Domain.Entities;
using DBS_Task.Infrastructure.Data.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DBS_Task.Application.CQRS.Products.Commands.ChangeProductStatus
{
    public class ChangeProductStatusCommandHandler : IRequestHandler<ChangeProductStatusCommand, ApiResponse<ChangeProductStatusResponseDto>>
    {
        private readonly ApplicationDbContext _dbContext;

        public ChangeProductStatusCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ApiResponse<ChangeProductStatusResponseDto>> Handle(ChangeProductStatusCommand request, CancellationToken cancellationToken)
        {
            // Validate product exists
            var product = await _dbContext.Products.FindAsync(request.ProductId, cancellationToken);

            if (product is null)
                return ApiResponse<ChangeProductStatusResponseDto>.FailureResponse($"Product with ID {request.ProductId} was not found.", 404);

            var statusChangeHistory = product.ChangeStatus(request.NewStatus);

            // Validate same status
            if (statusChangeHistory is null)
                return ApiResponse<ChangeProductStatusResponseDto>.FailureResponse($"Product is already set to '{request.NewStatus}'. No change was made.", 400);
            
            await _dbContext.SaveChangesAsync();

            var responseDto = new ChangeProductStatusResponseDto
            {
                ProductId = product.Id,
                OldStatus = statusChangeHistory.OldStatus,
                NewStatus = statusChangeHistory.NewStatus,
            };

            return ApiResponse<ChangeProductStatusResponseDto>.SuccessResponse(responseDto, 200, $"Product status updated to '{request.NewStatus}' successfully.");

        }
    }
}
