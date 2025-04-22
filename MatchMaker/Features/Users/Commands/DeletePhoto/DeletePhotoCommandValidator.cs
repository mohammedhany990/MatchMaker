using FluentValidation;

namespace MatchMaker.Features.Users.Commands.DeletePhoto
{
    
    public class DeletePhotoCommandValidator : AbstractValidator<DeletePhotoCommand>
    {
        public DeletePhotoCommandValidator()
        {
            RuleFor(x => x.PhotoId)
                .GreaterThan(0).WithMessage("Photo ID must be greater than zero.");

            RuleFor(x => x.UserEmail)
                .NotEmpty().WithMessage("User email is required.")
                .EmailAddress().WithMessage("Invalid email address format.");
        }
    }

}
