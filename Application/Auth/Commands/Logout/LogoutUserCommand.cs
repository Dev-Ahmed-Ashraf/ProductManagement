using DBS_Task.Application.Common.Results;
using DBS_Task.Contracts;
using MediatR;

namespace DBS_Task.Application.Auth.Commands.Logout
{
    public record LogoutUserCommand(RefreshTokenRequestContract RequestContract) : IRequest<ApiResponse<bool>>
    {
    }
}
