using DBS_Task.API.Responses;
using DBS_Task.Application.Common;
using DBS_Task.Application.Contracts;
using DBS_Task.Application.DTOs.ProductStatusHistory;
using DBS_Task.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DBS_Task.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductStatusHistoriesController : ControllerBase
    {
        private readonly IProductStatusHistoryService _productStatusHistoryService;

        public ProductStatusHistoriesController(IProductStatusHistoryService productStatusHistoryService)
        {
            _productStatusHistoryService = productStatusHistoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductStatusHistories([FromQuery] GetProductStatusHistoriesQueryContract query)
        {
            var histories = await _productStatusHistoryService.GetProductStatusHistoriesAsync(query);

            var response = ApiResponse<PaginatedResult<ProductStatusHistoriesResponseDto>>.SuccessResponse(histories, 200, "Product Status History Retrieved Successfully");
            return Ok(response);
        }
    }
}
