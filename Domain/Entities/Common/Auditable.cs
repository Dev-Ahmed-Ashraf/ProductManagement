namespace DBS_Task.Domain.Entities.Common
{
    public class AuditableEntity : BaseEntity
    {
        public DateTime CreatedAt { get; private set; }
        public string CreatedBy { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public string? UpdatedBy { get; private set; }


    }
}
