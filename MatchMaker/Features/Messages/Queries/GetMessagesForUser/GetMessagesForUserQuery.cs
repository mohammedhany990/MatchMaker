using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Messages.Queries.GetMessagesForUser
{
    public class GetMessagesForUserQuery : IRequest<PaginatedResponse<MessageDto>>
    {
        public MessageParams MessageParams { get; set; }
    }

}
