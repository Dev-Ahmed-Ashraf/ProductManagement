using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.User;
using DBS_Task.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace DBS_Task.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResponse<CreateUserResponseDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateUserCommandHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApiResponse<CreateUserResponseDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var existingEmail = await _userManager.FindByEmailAsync(request.UserContract.Email);
            if (existingEmail is not null)
            {
                return ApiResponse<CreateUserResponseDto>.FailureResponse("Email is already in use.", 400);
            }

            var existingRole = await _roleManager.RoleExistsAsync(request.UserContract.Role);
            if (!existingRole)
            {
                return ApiResponse<CreateUserResponseDto>.FailureResponse("Role does not exist.", 400);
            }

            var user = new ApplicationUser
            { 
                FullName = request.UserContract.FullName,
                Email = request.UserContract.Email,
                UserName = request.UserContract.Email,
            };

            var result = await _userManager.CreateAsync(user, request.UserContract.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<CreateUserResponseDto>.FailureResponse("Failed to create user.", 400, errors);
            }

            await _userManager.AddToRoleAsync(user, request.UserContract.Role);

            var responseDto = new CreateUserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = request.UserContract.Role,
                IsActive = true,
                CreatedAt = user.CreatedAt
            };
            return ApiResponse<CreateUserResponseDto>.SuccessResponse(responseDto, 201, "User created successfully.");
        }
    }
}
