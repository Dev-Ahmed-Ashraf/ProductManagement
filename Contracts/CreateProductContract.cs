namespace DBS_Task.Contracts
{
    public class CreateProductContract
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
