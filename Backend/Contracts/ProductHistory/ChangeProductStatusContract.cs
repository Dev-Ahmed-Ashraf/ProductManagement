using DBS_Task.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DBS_Task.Contracts.ProductHistory
{
    public class ChangeProductStatusContract
    {
        public ProductStatus newStatus { get; set; }
    }
}
