using DBS_Task.Application.Common.Interfaces;

namespace DBS_Task.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public string? UserId => "00000000-temp";
        public string? UserName => "Ahmed";
    }
}
