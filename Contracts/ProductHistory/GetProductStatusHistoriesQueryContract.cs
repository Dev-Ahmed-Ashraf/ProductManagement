using DBS_Task.Application.DTOs.Base;
using DBS_Task.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DBS_Task.Contracts.ProductHistory
{
    public class GetProductStatusHistoriesQueryContract : PaginationBaseDto
    {
        public int? ProductId { get; set; }
        public ProductStatus? OldStatus { get; set; }
        public ProductStatus? NewStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
