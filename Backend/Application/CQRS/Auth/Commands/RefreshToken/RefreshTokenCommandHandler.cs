using DBS_Task.Application.Common.Exceptions;
using DBS_Task.Application.Common.Interfaces;
using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Auth;
using DBS_Task.Infrastructure.Data.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DBS_Task.Application.CQRS.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<LoginResponseDto>>
    {
        private readonly IJWTService _jWTService;
        private readonly ApplicationDbContext _dbContext;

        public RefreshTokenCommandHandler(IJWTService jWTService, ApplicationDbContext dbContext)
        {
            _jWTService = jWTService;
            _dbContext = dbContext;
        }
        public async Task<ApiResponse<LoginResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var storedToken = await _dbContext.RefreshTokens
                               .Include(x => x.User)
                               .FirstOrDefaultAsync(x => x.Token == request.RequestContract.RefreshToken, cancellationToken);

            if (storedToken is null)
            
                throw new UnauthorizedException("Invalid refresh token");
            
            if (storedToken.IsRevoked || storedToken.IsExpired)
                throw new UnauthorizedException("Refresh token expired");

            // ROTATION
            storedToken.RevokedAt = DateTime.UtcNow;

            // issue new tokens
            var tokenResult = await _jWTService.GenerateTokenAsync(storedToken.User);

            await _dbContext.SaveChangesAsync(cancellationToken);


            var response =
               new LoginResponseDto
               {
                   userId = storedToken.User.Id,
                   fullName = storedToken.User.FullName,
                   email = storedToken.User.Email,
                   roles = tokenResult.Roles,
                   claims = tokenResult.Claims,
                   accessToken = tokenResult.AccessToken,
                   refreshToken = tokenResult.RefreshToken,
                   accessTokenExpiresAt = tokenResult.AccessTokenExpiresAt,
                   refreshTokenExpiresAt = tokenResult.RefreshTokenExpiresAt
               };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, 200, "Token refreshed successfully");
        }


    }
}
