using DBS_Task.Application.DTOs.Base;
using DBS_Task.Domain.Enums;
using System.Text.Json.Serialization;

namespace DBS_Task.Application.DTOs.ProductStatusHistory
{
    public class ProductStatusHistoriesResponseDto : AuditDto
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int ProductId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ProductName { get; set; }
        public ProductStatus OldStatus { get; set; }
        public ProductStatus NewStatus { get; set; }
    }
}
