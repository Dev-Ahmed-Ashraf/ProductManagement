using DBS_Task.Application.Common.Results;
using DBS_Task.Contracts.Auth;
using MediatR;

namespace DBS_Task.Application.CQRS.Auth.Commands.Logout
{
    public record LogoutUserCommand(RefreshTokenRequestContract RequestContract) : IRequest<ApiResponse<bool>>
    {
    }
}
