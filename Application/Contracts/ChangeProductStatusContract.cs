using DBS_Task.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DBS_Task.Application.Contracts
{
    public class ChangeProductStatusContract
    {
        [Required]
        [EnumDataType(typeof(ProductStatus), ErrorMessage = "Invalid value for Status.")]
        public ProductStatus Status { get; set; }
    }
}
