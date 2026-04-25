using FluentValidation;

namespace DBS_Task.Application.CQRS.Auth.Commands.Logout
{
    public class LogoutUserCommandValidator : AbstractValidator<LogoutUserCommand>
    {
        public LogoutUserCommandValidator()
        {
            RuleFor(x => x.RequestContract.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.");
        }
    }
}
