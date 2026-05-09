using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.User;
using DBS_Task.Contracts.User;
using MediatR;

namespace DBS_Task.Application.CQRS.Users.Commands.CreateUser
{
    public record CreateUserCommand(CreateUserContract UserContract) : IRequest<ApiResponse<CreateUserResponseDto>>
    {
    }
}
