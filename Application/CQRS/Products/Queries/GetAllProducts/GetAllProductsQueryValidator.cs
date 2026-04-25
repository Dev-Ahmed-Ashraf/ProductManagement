using FluentValidation;

namespace DBS_Task.Application.CQRS.Products.Queries.GetAllProducts
{
    public class GetAllProductsQueryValidator : AbstractValidator<GetAllProductsQuery>
    {
        public GetAllProductsQueryValidator()
        {
            RuleFor(x => x.ProductsQueryContract.Name)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.ProductsQueryContract.Name))
                .WithMessage("Product name cannot exceed 200 characters.");

            RuleFor(x => x.ProductsQueryContract.Description)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrWhiteSpace(x.ProductsQueryContract.Description))
                .WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.ProductsQueryContract.Price)
                .GreaterThan(0)
                .When(x => x.ProductsQueryContract.Price.HasValue)
                .WithMessage("Price must be greater than 0.");

            RuleFor(x => x.ProductsQueryContract.Quantity)
                .GreaterThanOrEqualTo(0)
                .When(x => x.ProductsQueryContract.Quantity.HasValue)
                .WithMessage("Quantity cannot be negative.");

            RuleFor(x => x.ProductsQueryContract.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.ProductsQueryContract.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100.");
        }
    }
}
