using DBS_Task.Application.DTOs.Product;
using DBS_Task.Domain.Enums;

namespace DBS_Task.Application.DTOs.ProductStatusHistory
{
    public class ProductStatusHistoriesResponseDto : ProductAuditDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public ProductStatus OldStatus { get; set; }
        public ProductStatus NewStatus { get; set; }
    }
}
