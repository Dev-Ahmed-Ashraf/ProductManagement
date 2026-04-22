using DBS_Task.Application.DTOs.Base;
using DBS_Task.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DBS_Task.Application.DTOs.Product
{
        public class ProductResponseDto : AuditDto
        {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public ProductStatus Status { get; set; }
    }
}
