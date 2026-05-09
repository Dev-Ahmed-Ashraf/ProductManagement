using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Auth;
using DBS_Task.Contracts.Auth;
using MediatR;

namespace DBS_Task.Application.CQRS.Auth.Commands.RefreshToken
{
    public record RefreshTokenCommand(RefreshTokenRequestContract RequestContract) : IRequest<ApiResponse<LoginResponseDto>>
    {
    }
}
