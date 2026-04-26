using DBS_Task.Application.Common.Constants;
using DBS_Task.Application.CQRS.Users.Commands.CreateUser;
using DBS_Task.Application.CQRS.Users.Queries.GetAllUsers;
using DBS_Task.Application.CQRS.Users.Queries.GetUserById;
using DBS_Task.Contracts.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DBS_Task.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Authorize(Policy = AppClaims.UsersCreate)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserContract contract)
        {
            var command = new CreateUserCommand(contract);
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Retrieves a paginated list of users.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        [Authorize(Policy = AppClaims.UsersView)]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersContract contract)
        {
            var query = new GetAllUsersQuery(contract);
            var result = await _mediator.Send(query);

            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Retrieves a user by the specified unique identifier.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        [Authorize(Policy = AppClaims.UsersView)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var query = new GetUserByIdQuery(id);
            var result = await _mediator.Send(query);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
