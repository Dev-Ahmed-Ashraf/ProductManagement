using DBS_Task.Application.DTOs.Base;

namespace DBS_Task.Contracts.User
{
    public class GetAllUsersContract : PaginationBaseDto
    {
         public string? Name { get; set; }
         public string? UserName { get; set; }
        public string? Email { get; set; }
         public string? Role { get; set; }
         public bool? IsActive { get; set; }
    }
}
