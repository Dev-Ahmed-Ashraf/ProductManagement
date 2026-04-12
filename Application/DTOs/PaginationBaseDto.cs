using System.ComponentModel.DataAnnotations;

namespace DBS_Task.Application.DTOs
{
    public class PaginationBaseDto
    {
        private const int _MaxPageSize = 100;
        private int _pageSize = 10;

        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > _MaxPageSize) ? _MaxPageSize : value;
        }
    }
}
