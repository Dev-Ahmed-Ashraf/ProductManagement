using DBS_Task.Application.Common;
using DBS_Task.Application.Contracts;
using DBS_Task.Application.DTOs.ProductStatusHistory;

namespace DBS_Task.Application.Interfaces
{
    public interface IProductStatusHistoryService
    {
        Task<PaginatedResult<ProductStatusHistoriesResponseDto>> GetProductStatusHistoriesAsync(GetProductStatusHistoriesQueryContract getProductStatusHistoriesQueryContract);
    }
}
