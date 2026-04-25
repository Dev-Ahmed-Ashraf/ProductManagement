using DBS_Task.Application.Common.Constants;
using DBS_Task.Application.Common.Results;
using DBS_Task.Application.CQRS.Statistics.Queries;
using DBS_Task.Application.DTOs.Statistics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DBS_Task.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StatisticsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StatisticsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieve application statistics. 
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<StatisticsResponseDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = AppClaims.StatisticsView)]
        public async Task<IActionResult> GetStatistics()
        {
            var query = new GetStatisticsQuery();
            var statistics = await _mediator.Send(query);
            return StatusCode((int)statistics.StatusCode, statistics);
        }
    }
}
