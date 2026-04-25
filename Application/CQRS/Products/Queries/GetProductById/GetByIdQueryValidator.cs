using FluentValidation;

namespace DBS_Task.Application.CQRS.Products.Queries.GetProductById
{
    public class GetByIdQueryValidator : AbstractValidator<GetByIdQuery>
    {
        public GetByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Product Id must be greater than 0.");
        }
    }
}
