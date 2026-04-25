namespace DBS_Task.Application.DTOs.Statistics
{
    public class StatisticsResponseDto
    {
        public ProductStatisticsDto Products { get; set; }
        public UserStatisticsDto Users { get; set; }
        public StatusChangeStatisticsDto StatusChanges { get; set; }
    }
}
