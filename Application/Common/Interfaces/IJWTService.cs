using DBS_Task.Application.Common.Results;
using DBS_Task.Domain.Entities;

namespace DBS_Task.Application.Common.Interfaces
{
    public interface IJWTService
    {
        Task<TokenResult> GenerateTokenAsync(ApplicationUser user);
    }
}
