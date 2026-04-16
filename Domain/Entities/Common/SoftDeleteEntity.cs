namespace DBS_Task.Domain.Entities.Common
{
    public class SoftDeleteEntity : AuditableEntity
    {
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedAt { get; private set; }
    }
}
