using System.ComponentModel.DataAnnotations;

namespace DBS_Task.Application.DTOs
{
    public class PaginationBaseDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
