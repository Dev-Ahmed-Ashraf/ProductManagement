using System.ComponentModel.DataAnnotations;

namespace DBS_Task.Application.Contracts
{
    public class CreateProductContract
    {
        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(200, ErrorMessage = "Product name cannot exceed 200 characters.")]
        public string Name { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; init; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; init; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int Quantity { get; init; }
    }
}
