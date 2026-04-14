
using DBS_Task.Domain.Enums;
using System.Runtime.CompilerServices;

namespace DBS_Task.Domain.Entities
{
    public class Product : SoftDeleteEntity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public ProductStatus Status { get; private set; } = ProductStatus.Available;

        private readonly List<ProductStatusHistory> _statusHistories = new();
        public IReadOnlyCollection<ProductStatusHistory> StatusHistories => _statusHistories.AsReadOnly();
        public Product(string name, string? description, decimal price, int quantity)
        {
            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
            
        }

        public ProductStatusHistory? ChangeStatus(ProductStatus newStatus)
        {
            if (Status == newStatus)
                return null;

            var oldStatus = Status;
            Status = newStatus;

            var statusistory =  new ProductStatusHistory(
                productId: Id,
                oldStatus: oldStatus,
                newStatus: newStatus
            );
            _statusHistories.Add(statusistory);
            return statusistory;
        }
    }
}
