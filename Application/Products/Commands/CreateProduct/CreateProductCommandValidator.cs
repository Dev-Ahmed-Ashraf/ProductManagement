using FluentValidation;

namespace DBS_Task.Application.Products.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters.");

            RuleFor(x => x.description)
                .MaximumLength(1000)
                .When(x => x.description != null)
                .WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0.");

            RuleFor(x => x.quantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quantity cannot be negative.");
        }
    }
}
