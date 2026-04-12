using DBS_Task.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace DBS_Task.Application.Contracts
{
    public class GetProductsQueryContract : PaginationBaseDto
    {
        [MaxLength(200, ErrorMessage = "Product name cannot exceed 200 characters.")]
        public string? Name { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; init; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal? Price { get; init; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int? Quantity { get; init; }
    }
}
