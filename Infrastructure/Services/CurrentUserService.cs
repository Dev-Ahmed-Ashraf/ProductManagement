using DBS_Task.Application.Common.Interfaces;
using System.Security.Claims;

namespace DBS_Task.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId =>
            _httpContextAccessor.HttpContext?.User?
                .FindFirstValue(ClaimTypes.NameIdentifier);

        public string? UserName =>
            _httpContextAccessor.HttpContext?.User?
                .FindFirstValue(ClaimTypes.Name);
    }
}
