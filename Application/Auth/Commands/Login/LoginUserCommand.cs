using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Auth;
using DBS_Task.Contracts;
using MediatR;

namespace DBS_Task.Application.Auth.Commands.Login
{
    public record LoginUserCommand(LoginRequestContract LoginRequest) : IRequest<ApiResponse<LoginResponseDto>>
    {
    }
}
