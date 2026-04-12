using System.ComponentModel.DataAnnotations;

namespace DBS_Task.Application.DTOs.Product
{
        public class ProductResponseDto : ProductAuditDto
        {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        }
}
