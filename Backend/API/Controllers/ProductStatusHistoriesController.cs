using DBS_Task.Application.Common.Constants;
using DBS_Task.Application.Common.Results;
using DBS_Task.Application.CQRS.ProductStatusHistories.Queries.GetAllStatusHistories;
using DBS_Task.Application.DTOs.ProductStatusHistory;
using DBS_Task.Contracts.ProductHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DBS_Task.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductStatusHistoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductStatusHistoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a paginated list of product status history records that match the specified filter criteria.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ProductStatusHistoriesResponseDto>>), StatusCodes.Status200OK)]
        [HttpGet]
        [Authorize(Policy = AppClaims.ProductStatusHistoriesView)]
        public async Task<IActionResult> GetProductStatusHistories([FromQuery] GetProductStatusHistoriesQueryContract request)
        {
            var query = new GetProductStatusHistoriesQuery(request);
            var response = await _mediator.Send(query);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
