using DBS_Task.API.Responses;
using DBS_Task.Application.Common;
using DBS_Task.Application.Contracts;
using DBS_Task.Application.DTOs.Product;
using DBS_Task.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DBS_Task.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Creates a new product using the specified product details.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductContract request)
        {
            var product = await _productService.CreateProductAsync(request);
            var response = ApiResponse<ProductResponseDto>.SuccessResponse(product, "Product created successfully.");
            return Created( "" , response);  
        }

        /// <summary>
        /// Retrieves a paginated list of products that match the specified query parameters.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ProductResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProducts([FromQuery] GetProductsQueryContract query)
        {
            var products = await _productService.GetAllProductsAsync(query);
            var response = ApiResponse<PaginatedResult<ProductResponseDto>>.SuccessResponse(products, "Products retrieved successfully.");
            return Ok(response);
        }

        /// <summary>
        /// Soft deletes a product by its ID
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.SoftDeleteProductAsync(id);

            if (!result)
                return NotFound(ApiResponse<object>.FailureResponse("Product not found or already deleted."));

            return Ok(ApiResponse<object>.SuccessResponse(null, "Product deleted successfully."));
        }
    }
}
