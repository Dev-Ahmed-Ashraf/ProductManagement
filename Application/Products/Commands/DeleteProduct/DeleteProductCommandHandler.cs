using DBS_Task.Application.Common.Results;
using DBS_Task.Infrastructure.Data.DBContext;
using MediatR;

namespace DBS_Task.Application.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ApiResponse<bool>>
    {
        private readonly ApplicationDbContext _dbContext;

        public DeleteProductCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _dbContext.Products.FindAsync(request.Id, cancellationToken);
            if (existingProduct == null || existingProduct.IsDeleted)
            {
                return ApiResponse<bool>.FailureResponse("Product not found or already deleted.", 404);
            }

            _dbContext.Products.Remove(existingProduct);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResponse(true, 200, "Product deleted successfully.");
        }
    }
}
