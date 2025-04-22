using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MatchMaker.Service.Abstracts;
using MediatR;

namespace MatchMaker.Features.Messages.Queries.GetMessageThread
{
    public class GetMessagesThreadQueryHandler : IRequestHandler<GetMessagesThreadQuery, BaseResponse<IEnumerable<MessageDto>>>
    {
        private readonly IMessageService _messageService;

        public GetMessagesThreadQueryHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<BaseResponse<IEnumerable<MessageDto>>> Handle(GetMessagesThreadQuery request, CancellationToken cancellationToken)
        {
            var result = await _messageService.GetMessagesThread(request.CurrentUsername, request.ReceiverUsername);
            return result; 
        }
    }

}
