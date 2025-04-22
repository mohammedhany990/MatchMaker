using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Messages.Commands.DeleteMessage
{
    public class DeleteMessageCommand : IRequest<BaseResponse<string>>
    {
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public DeleteMessageCommand(int messageId, int userId)
        {
            MessageId = messageId;
            UserId = userId;
        }
    }

}
