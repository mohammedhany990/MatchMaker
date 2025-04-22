using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Messages.Queries.GetMessageThread
{
    public class GetMessagesThreadQuery : IRequest<BaseResponse<IEnumerable<MessageDto>>>
    {
        public string CurrentUsername { get; set; }
        public string ReceiverUsername { get; set; }
        public GetMessagesThreadQuery(string currentUsername, string receiverUsername)
        {
            CurrentUsername = currentUsername;
            ReceiverUsername = receiverUsername;
        }
    }

}
