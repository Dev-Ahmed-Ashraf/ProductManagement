using DBS_Task.Application.Common.Interfaces;
using DBS_Task.Application.Common.Results;
using DBS_Task.Infrastructure.Data.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DBS_Task.Application.CQRS.Auth.Commands.Logout
{
    public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, ApiResponse<bool>>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUser;

        public LogoutUserCommandHandler(ApplicationDbContext dbContext, ICurrentUserService currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }
        public async Task<ApiResponse<bool>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            var token = await _dbContext.RefreshTokens.FirstOrDefaultAsync( x => x.Token == request.RequestContract.RefreshToken, cancellationToken);

            var userId = _currentUser.UserId;
            if (token.UserId != userId)
                return ApiResponse<bool>.FailureResponse("Unauthorized", 401);

            if (token is null)
                return ApiResponse<bool>.FailureResponse("Refresh token not found", 404);

            if (token.IsRevoked)
                return ApiResponse<bool>.SuccessResponse(true, 200, "Already logged out");

            token.RevokedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResponse( true, 200, "Logged out successfully");
        }
    }
}
