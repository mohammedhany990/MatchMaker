using FluentValidation;
using MatchMaker.Core.DTOs;

namespace MatchMaker.Features.Authentication.Commands.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("New password is required.")
                .MinimumLength(6)
                .WithMessage("New password must be at least 6 characters long.");

            RuleFor(x => x.Otp)
                .NotEmpty()
                .WithMessage("OTP is required.")
                .Matches(@"^\d{6}$").WithMessage("OTP must be a 6-digit number.");
        }
    }
}
