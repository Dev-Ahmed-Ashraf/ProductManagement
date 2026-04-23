using DBS_Task.Application.Auth.Commands;
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
    }
}
