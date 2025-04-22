using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Messages.Commands
{
    public class CreateMessageCommand : IRequest<BaseResponse<MessageDto>>
    {
        public string RecipientUsername { get; set; }
        public string Content { get; set; }

        public CreateMessageCommand(string recipientUsername, string content)
        {
            Content = content;
            RecipientUsername = recipientUsername;
        }

    }

}
