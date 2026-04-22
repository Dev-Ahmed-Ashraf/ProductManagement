using DBS_Task.Application.Roles.Queries.GetAllRoles;
using DBS_Task.Application.Roles.Queries.GetRoleClaims;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DBS_Task.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllRoles()
        {
            var query = new GetAllRolesQuery();
            var result = await _mediator.Send(query);

            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Retrieves the list of claims associated with the specified role.
        /// </summary>
        /// <param name="id">The unique identifier of the role for which to retrieve claims.</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}/claims")]
        public async Task<IActionResult> GetRoleClaims(string id)
        {
            var query = new GetRoleClaimsQuery(id);
            var result = await _mediator.Send(query);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}
