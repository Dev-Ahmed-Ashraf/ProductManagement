using FluentValidation;

namespace DBS_Task.Application.ProductStatusHistories.Queries.GetAllStatusHistories
{
    public class GetProductStatusHistoriesQueryValidator : AbstractValidator<GetProductStatusHistoriesQuery>
    {
        public GetProductStatusHistoriesQueryValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0).When(x => x.ProductId.HasValue)
                .WithMessage("ProductId must be greater than 0.");

            RuleFor(X => X.OldStatus).IsInEnum().When(x => x.OldStatus.HasValue)
                .WithMessage("OldStatus must be a valid ProductStatus enum value.");

            RuleFor(X => X.NewStatus).IsInEnum().When(x => x.NewStatus.HasValue)
                .WithMessage("NewStatus must be a valid ProductStatus enum value.");

            RuleFor(x => x.FromDate).LessThanOrEqualTo(x => x.ToDate).When(x => x.FromDate.HasValue && x.ToDate.HasValue)
                .WithMessage("FromDate must be less than or equal to ToDate.");

            RuleFor(X => X.ToDate).GreaterThanOrEqualTo(X => X.FromDate).When(x => x.ToDate.HasValue && x.FromDate.HasValue)
                .WithMessage("ToDate must be greater than or equal to FromDate.");

            RuleFor(x => x.PageNumber).GreaterThan(0)
                .WithMessage("PageNumber must be greater than 0.");

            RuleFor(x => x.PageSize).InclusiveBetween(1, 100)
                .WithMessage("PageSize must be between 1 and 100.");
        }
    }
}
