using DBS_Task.Domain.Entities.Common;
using DBS_Task.Domain.Enums;

namespace DBS_Task.Domain.Entities
{
    public class ProductStatusHistory : AuditableEntity
    {
        public int ProductId { get; private set; }
        public ProductStatus OldStatus { get; private set; }
        public ProductStatus NewStatus { get; private set; }
        public Product Product { get; private set; }

        public ProductStatusHistory(int productId, ProductStatus oldStatus, ProductStatus newStatus) : base()
        {
            ProductId = productId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }
    }
}
