using DBS_Task.Application.Common.Exceptions;
using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.User;
using DBS_Task.Infrastructure.Data.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DBS_Task.Application.CQRS.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ApiResponse<UserResponseDto>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetUserByIdQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ApiResponse<UserResponseDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var result = _dbContext.UserWithRoles.Where(ur => ur.Id == request.UserId).FirstOrDefault();

            if (result is null)
            {
                throw new NotFoundException($"User with ID {request.UserId} not found.");
            }

            var userResponse = new UserResponseDto
            {
                Id = result.Id,
                FullName = result.FullName,
                Email = result.Email,
                UserName = result.UserName,
                IsActive = result.IsActive,
                CreatedAt = result.CreatedAt,
                Role = result.Role
            };

            return ApiResponse<UserResponseDto>.SuccessResponse(userResponse, 200, "User retrieved successfully.");

        }
    }
}
