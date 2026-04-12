
namespace DBS_Task.Domain.Entities
{
    public class Product : SoftDeleteEntity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }

        public Product(string name, string? description, decimal price, int quantity)
        {
            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
        }
    }
}
