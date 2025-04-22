using MatchMaker.Core.Helper;
using MatchMaker.Service.Abstracts;
using MediatR;

namespace MatchMaker.Features.Messages.Commands.DeleteMessage
{
   
    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, BaseResponse<string>>
    {
        private readonly IMessageService _messageService;

        public DeleteMessageCommandHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<BaseResponse<string>> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            return await _messageService.DeleteMessageAsync(request.MessageId, request.UserId);
        }
    }
}
