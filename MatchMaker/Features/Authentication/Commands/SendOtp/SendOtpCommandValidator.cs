using FluentValidation;

namespace MatchMaker.Features.Authentication.Commands.SendOtp
{
    public class SendOtpCommandValidator : AbstractValidator<SendOtpCommand>
    {
        public SendOtpCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");
        }
    }
}
