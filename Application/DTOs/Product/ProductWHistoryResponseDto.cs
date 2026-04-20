using DBS_Task.Application.DTOs.ProductStatusHistory;

namespace DBS_Task.Application.DTOs.Product
{
    public class ProductWHistoryResponseDto : ProductResponseDto
    {
        public List<ProductStatusHistoriesResponseDto> History { set; get; }
    }
}
