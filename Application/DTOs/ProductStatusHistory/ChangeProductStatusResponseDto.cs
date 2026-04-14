using DBS_Task.Domain.Enums;

namespace DBS_Task.Application.DTOs.Product
{
    public class ChangeProductStatusResponseDto
    {
        public int ProductId { get; set; }
        public ProductStatus OldStatus { get; set; }
        public ProductStatus NewStatus { get; set; }
    }
}
