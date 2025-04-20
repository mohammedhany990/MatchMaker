using AutoMapper;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.ExtensionMethods;
using MatchMaker.Service.Abstracts;
using MatchMaker.Service.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MatchMaker.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;

        public MessageHub(IMessageService messageService,
            UserManager<AppUser> userManager,
            IMapper mapper,
            IHubContext<PresenceHub> presenceHub)
        {
            _messageService = messageService;
            _userManager = userManager;
            _mapper = mapper;
            _presenceHub = presenceHub;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext is null)
            {
                throw new HubException("Not logged in");
            }
            var otherUser = httpContext?.Request.Query["user"];

            if (string.IsNullOrEmpty(otherUser) || Context.User is null)
            {
                throw new Exception("Cannot join group.");
            }

            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var group = await AddToGroup(groupName);

            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var message = await _messageService.GetMessagesThread(Context.User.GetUsername(), otherUser!);

            await Clients.Caller.SendAsync("ReceiveMessageThread", message);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);

            await base.OnDisconnectedAsync(exception);
        }


        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User?.GetUsername() ?? throw new Exception("Couldn't get user.");

            if (username == createMessageDto.RecipientUsername.ToLower())
            {
                throw new HubException("You cannot send a message to yourself.");
            }

            var sender = await _userManager.Users
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.UserName == username);

            var recipient = await _userManager.Users
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.UserName == createMessageDto.RecipientUsername.ToLower());

            if (sender == null || recipient == null || sender.UserName == null || recipient.UserName == null)
            {
                throw new HubException("Message cannot be sent at this time.");
            }

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);

            var group = await _messageService.GetMessageGroup(groupName);

            if (group != null && group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await PresenceTracker.GetConnectionsForUSer(recipient.UserName);
                if (connections != null && connections?.Count() > 0)
                {
                    await _presenceHub.Clients
                        .Clients(connections)
                        .SendAsync("NewMessageReceived", new
                        {
                            username = sender.UserName,
                            KnownAs = sender.KnownAs
                        });
                }
            }

            await _messageService.AddMessage(message);

            if (await _messageService.SaveAllAsync())
            {
                await Clients
                    .Group(groupName)
                    .SendAsync("NewMessage"
                        , message
                        , _mapper.Map<MessageDto>(message));

            }

        }

        private async Task<Group> AddToGroup(string groupName)
        {
            var username = Context.User?.GetUsername() ?? throw new Exception("Couldn't get user.");
            var connection = new Connection
            {
                ConnectionId = Context.ConnectionId,
                Username = username

            };
            var group = await _messageService.GetMessageGroup(groupName);
            if (group == null)
            {
                group = new Group
                {
                    Name = groupName
                };
                await _messageService.AddGroup(group);
            }
            group.Connections.Add(connection);

            if (await _messageService.SaveAllAsync())
            {
                return group;
            }
            throw new HubException("Failed to add to group.");
        }
        
        private async Task RemoveFromGroup(string groupName)
        {
            var connection = await _messageService.GetConnection(Context.ConnectionId);
            if (connection != null)
            {
                _messageService.RemoveConnection(connection);
            }
        }
        
        private async Task<Group> RemoveFromMessageGroup()
        {
            var groups = await _messageService.GetGroupForConnections(Context.ConnectionId);
            var connection = groups?.Connections.FirstOrDefault(x=> x.ConnectionId == Context.ConnectionId);
            if (connection != null && groups != null)
            {
                _messageService.RemoveConnection(connection);
                if (await _messageService.SaveAllAsync())
                {
                    return groups;
                }
            }

            throw new HubException("Failed to remove from group.");
        }


        private string GetGroupName(string caller, string? otherUser)
        {
            var stringCompare = string.CompareOrdinal(caller, otherUser) < 0;
            if (stringCompare)
            {
                return $"{caller}-{otherUser}";
            }
            else
            {
                return $"{otherUser}-{caller}";
            }
        }
    }
}
