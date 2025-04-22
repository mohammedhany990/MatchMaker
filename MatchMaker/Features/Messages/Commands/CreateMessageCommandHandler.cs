using AutoMapper;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using MatchMaker.ExtensionMethods;
using MatchMaker.Service.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MatchMaker.Features.Messages.Commands
{
    public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, BaseResponse<MessageDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMessageService _messageService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateMessageCommandHandler(UserManager<AppUser> userManager, IMessageService messageService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _messageService = messageService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BaseResponse<MessageDto>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            var senderUsername = _httpContextAccessor.HttpContext?.User.GetEmail();
            if (senderUsername == request.RecipientUsername.ToLower())
            {
                return new BaseResponse<MessageDto>(StatusCodes.Status400BadRequest, false, null, "You cannot send a message to yourself");
            }

            var sender = await _userManager.Users
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.UserName == senderUsername);

            var recipient = await _userManager.Users
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.UserName == request.RecipientUsername.ToLower());

            if (sender == null || recipient == null || sender.UserName == null || recipient.UserName == null)
            {
                return new BaseResponse<MessageDto>(StatusCodes.Status404NotFound, false, null, "Message cannot be sent at this time.");
            }

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = request.Content
            };

            await _messageService.AddMessage(message);

            if (await _messageService.SaveAllAsync())
            {
                var msgDto = _mapper.Map<MessageDto>(message);
                return new BaseResponse<MessageDto>(StatusCodes.Status200OK, true, 1, msgDto, "Message sent successfully");
            }

            return new BaseResponse<MessageDto>(StatusCodes.Status400BadRequest, false, null, "Failed to add message");
        }
    }

}
