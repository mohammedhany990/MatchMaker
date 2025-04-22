using FluentValidation;

namespace MatchMaker.Features.Users.Commands.SetMainPhoto
{
    public class SetMainPhotoCommandValidator : AbstractValidator<SetMainPhotoCommand>
    {
        public SetMainPhotoCommandValidator()
        {
            RuleFor(x => x.UserEmail)
                .NotEmpty().WithMessage("User email is required.")
                .EmailAddress().WithMessage("Invalid email address format.");

            RuleFor(x => x.PhotoId)
                .GreaterThan(0).WithMessage("Photo ID must be greater than zero.");
        }
    }
}
