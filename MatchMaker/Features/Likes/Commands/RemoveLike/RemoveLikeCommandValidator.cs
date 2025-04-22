using FluentValidation;

namespace MatchMaker.Features.Likes.Commands.RemoveLike
{
    public class RemoveLikeCommandValidator : AbstractValidator<RemoveLikeCommand>
    {
        public RemoveLikeCommandValidator()
        {
            RuleFor(x => x.SourceUserId)
                .GreaterThan(0).WithMessage("SourceUserId must be greater than 0.");

            RuleFor(x => x.TargetUserId)
                .GreaterThan(0).WithMessage("TargetUserId must be greater than 0.");

            RuleFor(x => x)
                .Must(x => x.SourceUserId != x.TargetUserId)
                .WithMessage("You cannot remove a like from yourself.");
        }
    }
}
