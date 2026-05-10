using DBS_Task.Application.Common.Results;
using DBS_Task.Application.Common.Exceptions;
using DBS_Task.Application.DTOs.User;
using DBS_Task.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace DBS_Task.Application.CQRS.Users.Commands.CreateUser
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
                throw new BadRequestException("Email is already in use.");
            }

            var existingRole = await _roleManager.RoleExistsAsync(request.UserContract.Role);
            if (!existingRole)
            {
                throw new BadRequestException($"Role '{request.UserContract.Role}' does not exist.");
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
                throw new BadRequestException("Failed to create user.");
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
