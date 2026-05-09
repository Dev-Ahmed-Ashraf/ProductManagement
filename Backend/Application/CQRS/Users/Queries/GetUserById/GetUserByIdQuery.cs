using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.User;
using MediatR;

namespace DBS_Task.Application.CQRS.Users.Queries.GetUserById
{
    public record GetUserByIdQuery(string UserId) : IRequest<ApiResponse<UserResponseDto>>
    {
    }
}
