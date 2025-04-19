using AutoMapper;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using MatchMaker.ExtensionMethods;
using MatchMaker.Service.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MatchMaker.Controllers
{
    [Authorize]
    public class MessagesController : ApiBaseController
    {
        private readonly IMessageService _messageService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public MessagesController(IMessageService messageService, UserManager<AppUser> userManager, IMapper mapper)
        {
            _messageService = messageService;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpPost("add-message")]
        public async Task<ActionResult> CreateMessage([FromBody] CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
            {
                return BadRequest("You cannot send a message to yourself");
            }

            var sender = await _userManager.Users
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.UserName == username);

            var recipient = await _userManager.Users
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.UserName == createMessageDto.RecipientUsername.ToLower());

            if (sender == null || recipient == null || sender.UserName == null || recipient.UserName == null)
            {
                return NotFound("Message cannot be sent at this time.");
            }

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            await _messageService.AddMessage(message);

            if (await _messageService.SaveAllAsync())
            {
                var msgDto = _mapper.Map<MessageDto>(message);
                return Ok(msgDto);
            }

            return BadRequest("Failed to add message");
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<MessageDto>>> GetMessagesForUser(
            [FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            var messages = await _messageService.GetMessageForUser(messageParams);
            Response.AddPaginationHeader(messages);

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesThread(string username)
        {
            var currentUsername = User.GetUsername();
            var messages = await _messageService.GetMessagesThread(currentUsername, username);
            return Ok(messages);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var userId = User.GetId();
            var result = await _messageService.DeleteMessage(id, userId);
            return BadRequest(result);

        }
    }
}
