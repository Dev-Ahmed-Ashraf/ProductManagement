using DBS_Task.Application.Auth.Commands;
using DBS_Task.Application.Auth.Commands.Login;
using DBS_Task.Application.Auth.Commands.Logout;
using DBS_Task.Application.Auth.Commands.RefreshToken;
using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Auth;
using DBS_Task.Contracts;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DBS_Task.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Authenticates a user and returns a JWT token if the credentials are valid.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status403Forbidden)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestContract request)
        {
            var command = new LoginUserCommand(request);
            var result = await _mediator.Send(command);

            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Handles a request to refresh an authentication token.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status400BadRequest)]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestContract requestContract)
        {
            var command = new RefreshTokenCommand(requestContract);
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Logs out the user by invalidating the specified refresh token.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestContract requestContract)
        {
            var command = new LogoutUserCommand(requestContract);
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
