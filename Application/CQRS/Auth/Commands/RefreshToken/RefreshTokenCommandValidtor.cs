using FluentValidation;

namespace DBS_Task.Application.CQRS.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandValidtor : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidtor()
        {
            RuleFor(x => x.RequestContract.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.");
        }
    }
}
