namespace DBS_Task.Application.DTOs.Product
{
    public class ProductAuditDto : ProductBaseDto
    {
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        
    }
}
