using FluentValidation;

namespace DBS_Task.Application.CQRS.ProductStatusHistories.Queries.GetAllStatusHistories
{
    public class GetProductStatusHistoriesQueryValidator : AbstractValidator<GetProductStatusHistoriesQuery>
    {
        public GetProductStatusHistoriesQueryValidator()
        {
            RuleFor(x => x.HistoriesQueryContract.ProductId).GreaterThan(0).When(x => x.HistoriesQueryContract.ProductId.HasValue)
                .WithMessage("ProductId must be greater than 0.");

            RuleFor(X => X.HistoriesQueryContract.OldStatus).IsInEnum().When(x => x.HistoriesQueryContract.OldStatus.HasValue)
                .WithMessage("OldStatus must be a valid ProductStatus enum value.");

            RuleFor(X => X.HistoriesQueryContract.NewStatus).IsInEnum().When(x => x.HistoriesQueryContract.NewStatus.HasValue)
                .WithMessage("NewStatus must be a valid ProductStatus enum value.");

            RuleFor(x => x.HistoriesQueryContract.FromDate).LessThanOrEqualTo(x => x.HistoriesQueryContract.ToDate).When(x => x.HistoriesQueryContract.FromDate.HasValue && x.HistoriesQueryContract.ToDate.HasValue)
                .WithMessage("FromDate must be less than or equal to ToDate.");

            RuleFor(X => X.HistoriesQueryContract.ToDate).GreaterThanOrEqualTo(X => X.HistoriesQueryContract.FromDate).When(x => x.HistoriesQueryContract.ToDate.HasValue && x.HistoriesQueryContract.FromDate.HasValue)
                .WithMessage("ToDate must be greater than or equal to FromDate.");

            RuleFor(x => x.HistoriesQueryContract.PageNumber).GreaterThan(0)
                .WithMessage("PageNumber must be greater than 0.");

            RuleFor(x => x.HistoriesQueryContract.PageSize).InclusiveBetween(1, 100)
                .WithMessage("PageSize must be between 1 and 100.");
        }
    }
}
