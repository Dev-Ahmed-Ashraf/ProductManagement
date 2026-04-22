using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.User;
using DBS_Task.Infrastructure.Data.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DBS_Task.Application.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ApiResponse<PaginatedResult<UserResponseDto>>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetAllUsersQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<PaginatedResult<UserResponseDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
                var Contract = request.contract; 

            var query = _dbContext.UserWithRoles.AsNoTracking();

            // Filters
            if (!string.IsNullOrWhiteSpace(Contract.Name))
            {
                query = query.Where(u => u.FullName.Contains(Contract.Name));
            }
            if (!string.IsNullOrWhiteSpace(Contract.UserName))
            {
                query = query.Where(u => u.UserName.Contains(Contract.UserName));
            }
            if (!string.IsNullOrWhiteSpace(Contract.Email))
            {
                query = query.Where(u => u.Email.Contains(Contract.Email));
            }
            if (Contract.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == Contract.IsActive.Value);
            }
            if (!string.IsNullOrWhiteSpace(Contract.Role))
            {
                query = query.Where(u => u.Role == Contract.Role);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            // Pagination and Projection
            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((Contract.PageNumber - 1) * Contract.PageSize)
                .Take(Contract.PageSize)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    UserName = u.UserName,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    Role = u.Role
                })
                .ToListAsync(cancellationToken);

            var paginatedResult = new PaginatedResult<UserResponseDto>
            (
               users,
               Contract.PageNumber,
               Contract.PageSize,
               totalCount
            );

            return ApiResponse<PaginatedResult<UserResponseDto>>.
                SuccessResponse(paginatedResult, 200, $"Successfully retrieved {users.Count} users.");
        }
    }
}
