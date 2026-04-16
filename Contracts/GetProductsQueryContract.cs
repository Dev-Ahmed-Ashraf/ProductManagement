using DBS_Task.Application.DTOs;

namespace DBS_Task.Contracts
{
    public class GetProductsQueryContract : PaginationBaseDto
    {
        public string? Name { get; set; }
        public string? Description { get; init; }
        public decimal? Price { get; init; }
        public int? Quantity { get; init; }
    }
}
