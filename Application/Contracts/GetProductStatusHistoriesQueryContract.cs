using DBS_Task.Application.DTOs;
using DBS_Task.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DBS_Task.Application.Contracts
{
    public class GetProductStatusHistoriesQueryContract : PaginationBaseDto, IValidatableObject
    {
        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be greater than 0.")]
        public int? ProductId { get; set; }

        [EnumDataType(typeof(ProductStatus), ErrorMessage = "Invalid value for OldStatus.")]
        public ProductStatus? OldStatus { get; set; }

        [EnumDataType(typeof(ProductStatus), ErrorMessage = "Invalid value for NewStatus.")]
        public ProductStatus? NewStatus { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format for FromDate.")]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format for ToDate.")]
        public DateTime? ToDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FromDate.HasValue && ToDate.HasValue && FromDate > ToDate)
            {
                yield return new ValidationResult(
                    "FromDate cannot be later than ToDate.",
                    new[] { nameof(FromDate) });
            }
        }
    }
}
