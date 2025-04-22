using FluentValidation;

namespace MatchMaker.Features.Messages.Commands
{
    public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
    {
        public CreateMessageCommandValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Message content cannot be empty.");
        }
    }

}
