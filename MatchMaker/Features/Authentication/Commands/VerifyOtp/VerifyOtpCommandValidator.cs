using FluentValidation;

namespace MatchMaker.Features.Authentication.Commands.VerifyOtp
{
    public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
    {
        public VerifyOtpCommandValidator()
        {

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.");

            RuleFor(x => x.Otp)
                .NotEmpty()
                .WithMessage("Otp is required.");
        }
    }
}
