using FluentValidation;

namespace DBS_Task.Application.Products.Commands.ChangeProductStatus
{
    public class ChangeProductStatusCommandValidator : AbstractValidator<ChangeProductStatusCommand>
    {
        public ChangeProductStatusCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product ID must be greater than 0.");

            RuleFor(x => x.NewStatus)
                .IsInEnum().WithMessage("Invalid product status value.");
        }
    }
}
