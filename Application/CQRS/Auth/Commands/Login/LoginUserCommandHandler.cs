using DBS_Task.Application.Common.Interfaces;
using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Auth;
using DBS_Task.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DBS_Task.Application.CQRS.Auth.Commands.Login
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, ApiResponse<LoginResponseDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJWTService _jWTService;

        public LoginUserCommandHandler(
            UserManager<ApplicationUser> userManager, 
            IJWTService jWTService)
        {
            _userManager = userManager;
            _jWTService = jWTService;
        }

        public async Task<ApiResponse<LoginResponseDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.LoginRequest.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, request.LoginRequest.Password)) 
                return ApiResponse<LoginResponseDto>.FailureResponse("Invalid email or password.", 401);
        
            if(!user.IsActive)
                return ApiResponse<LoginResponseDto>.FailureResponse("User account is inactive. Please contact support.", 403);

            var result = await _jWTService.GenerateTokenAsync(user);
                           
            var response = new LoginResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Roles = result.Roles,
                Claims = result.Claims,
                AccessToken = result.AccessToken,
                AccessTokenExpiresAt = result.AccessTokenExpiresAt,
                RefreshToken = result.RefreshToken,
                RefreshTokenExpiresAt = result.RefreshTokenExpiresAt
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, 200, "User logged in successfully.");
        }
    }
}
