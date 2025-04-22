using FluentValidation;

namespace MatchMaker.Features.Likes.Commands.AddLike
{
    public class AddLikeCommandValidator : AbstractValidator<AddLikeCommand>
    {
        public AddLikeCommandValidator()
        {
            RuleFor(x => x.SourceUserId).GreaterThan(0);
            RuleFor(x => x.TargetUserId).GreaterThan(0);
            RuleFor(x => x.TargetUserId).NotEqual(x => x.SourceUserId)
                .WithMessage("You cannot like yourself.");
        }
    }
}
