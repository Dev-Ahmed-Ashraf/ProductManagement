namespace DBS_Task.Application.DTOs.Statistics
{
    public class ProductStatisticsDto
    {
        public int Total { get; set; }
        public int Available { get; set; }
        public int OutOfStock { get; set; }
        public int Discontinued { get; set; }
        public int PreOrder { get; set; }
        public int BackOrder { get; set; }
        public int Draft { get; set; }
    }
}
