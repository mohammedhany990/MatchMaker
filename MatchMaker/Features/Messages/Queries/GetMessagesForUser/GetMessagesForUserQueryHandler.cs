using AutoMapper;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MatchMaker.Service.Abstracts;
using MediatR;

namespace MatchMaker.Features.Messages.Queries.GetMessagesForUser
{
    public class GetMessagesForUserQueryHandler : IRequestHandler<GetMessagesForUserQuery, PaginatedResponse<MessageDto>>
    {
        private readonly IMessageService _messageService;
        private readonly IMapper _mapper;

        public GetMessagesForUserQueryHandler(IMessageService messageService, IMapper mapper)
        {
            _messageService = messageService;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<MessageDto>> Handle(GetMessagesForUserQuery request, CancellationToken cancellationToken)
        {
            var messageParams = request.MessageParams;

            var pagedMessages = await _messageService.GetMessageForUser(messageParams);

            return pagedMessages;
        }
    }

}
