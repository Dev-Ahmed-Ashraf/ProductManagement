using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.User;
using DBS_Task.Contracts;
using MediatR;

namespace DBS_Task.Application.Users.Queries.GetAllUsers
{
    public record GetAllUsersQuery(GetAllUsersContract contract)
        : IRequest<ApiResponse<PaginatedResult<UserResponseDto>>>
    {

    }
}
