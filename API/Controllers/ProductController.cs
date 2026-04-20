using DBS_Task.Application.Common.Results;
using DBS_Task.Application.DTOs.Product;
using DBS_Task.Application.DTOs.ProductStatusHistory;
using DBS_Task.Application.Products.Commands.ChangeProductStatus;
using DBS_Task.Application.Products.Commands.CreateProduct;
using DBS_Task.Application.Products.Commands.DeleteProduct;
using DBS_Task.Application.Products.Queries.GetAllProducts;
using DBS_Task.Application.Products.Queries.GetProductById;
using DBS_Task.Contracts;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DBS_Task.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new product using the specified product details.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var product = await _mediator.Send(command);
            return StatusCode((int)product.StatusCode, product);
        }

        /// <summary>
        /// Retrieves a paginated list of products that match the specified query parameters.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ProductResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProducts([FromQuery] GetProductsQueryContract query)
        {
            var filters = new GetAllProductsQuery(query);

            var products = await _mediator.Send(filters);
            return StatusCode((int)products.StatusCode, products);
        }

        /// <summary>   
        /// Retrieves a product and its history by the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product to retrieve.</param>
        [ProducesResponseType(typeof(ApiResponse<ProductWHistoryResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ProductWHistoryResponseDto>), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var query = new GetByIdQuery(id);
            var product = await _mediator.Send(query);
            return StatusCode((int)product.StatusCode, product);
        }

        /// <summary>
        /// Soft deletes a product by its ID
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var command = new DeleteProductCommand(id);
            var result = await _mediator.Send(command);

            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Updates the status of the specified product.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<ChangeProductStatusResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ChangeProductStatusResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ChangeProductStatusResponseDto>), StatusCodes.Status404NotFound)]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeProductStatus([FromRoute] int id, [FromBody] ChangeProductStatusContract request)
        {
            var command = new ChangeProductStatusCommand(id, request.newStatus);
            var result = await _mediator.Send(command);

            return StatusCode((int)result.StatusCode, result);  
        }
    }
}
