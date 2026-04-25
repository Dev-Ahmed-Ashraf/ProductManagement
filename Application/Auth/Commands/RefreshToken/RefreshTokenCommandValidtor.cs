using FluentValidation;

namespace DBS_Task.Application.Auth.Commands.RefreshToken
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
