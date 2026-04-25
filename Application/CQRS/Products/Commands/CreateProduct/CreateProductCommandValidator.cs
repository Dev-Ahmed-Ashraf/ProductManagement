using FluentValidation;

namespace DBS_Task.Application.CQRS.Products.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.CreateProductContract.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters.");

            RuleFor(x => x.CreateProductContract.Description)
                .MaximumLength(1000)
                .When(x => x.CreateProductContract.Description != null)
                .WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.CreateProductContract.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0.");

            RuleFor(x => x.CreateProductContract.Quantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quantity cannot be negative.");
        }
    }
}
