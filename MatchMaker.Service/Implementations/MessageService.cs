﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using MatchMaker.Infrastructure.Interfaces;
using MatchMaker.Service.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MatchMaker.Service.Implementations
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MessageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task AddMessage(Message message)
        {
            await _unitOfWork.Repository<Message, int>().AddAsync(message);
        }

        //public async Task<string> DeleteMessage(int messageId, int userId)
        //{
        //    if (userId <= 0 || userId == null)
        //    {
        //        return "User ID cannot be empty";
        //    }

        //    if (messageId <= 0)
        //    {
        //        return "Invalid message ID";
        //    }
        //    var user = await _unitOfWork.Repository<AppUser, int>().GetAsync(userId);
        //    if (user is null)
        //    {
        //        return "User not found";
        //    }


        //    var message = await _unitOfWork.Repository<Message, int>().GetAsync(messageId);
        //    if (message is null)
        //    {
        //        return "Message not found";
        //    }
        //    if (message.SenderUsername != user.UserName && message.RecipientUsername != user.UserName)
        //    {
        //        return "You cannot delete this message";
        //    }

        //    if (message.SenderUsername == user.UserName)
        //    {
        //        message.SenderDeleted = true;
        //    }
        //    if (message.RecipientUsername == user.UserName)
        //    {
        //        message.RecipientDeleted = true;
        //    }

        //    if (message.SenderDeleted && message.RecipientDeleted)
        //    {
        //        _unitOfWork.Repository<Message, int>().Delete(message);
        //    }

        //    await _unitOfWork.SaveAsync();

        //    return message.SenderDeleted && message.RecipientDeleted
        //            ? "Message permanently deleted"
        //            : "Message deleted from your inbox";

        //}
        public async Task<BaseResponse<string>> DeleteMessageAsync(int messageId, int userId)
        {
            var user = await _unitOfWork.Repository<AppUser, int>().GetAsync(userId);
            if (user is null)
            {
                return new BaseResponse<string>(404, false, "User not found");
            }

            var message = await _unitOfWork.Repository<Message, int>().GetAsync(messageId);
            if (message is null)
            {
                return new BaseResponse<string>(404, false, "Message not found");
            }

            if (message.SenderUsername != user.UserName && message.RecipientUsername != user.UserName)
            {
                return new BaseResponse<string>(403, false, "You cannot delete this message");
            }

            if (message.SenderUsername == user.UserName)
            {
                message.SenderDeleted = true;
            }
            if (message.RecipientUsername == user.UserName)
            {
                message.RecipientDeleted = true;
            }

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _unitOfWork.Repository<Message, int>().Delete(message);
            }

            await _unitOfWork.SaveAsync();

            var resultMessage = message.SenderDeleted && message.RecipientDeleted
                ? "Message permanently deleted"
                : "Message deleted from your inbox";

            return new BaseResponse<string>(200, true, resultMessage);
        }
        public async Task<Message?> GetMessageByIdAsync(int id)
        {
            return await _unitOfWork.Repository<Message, int>().GetAsync(id);
        }



        public async Task<PaginatedResponse<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
            var query = _unitOfWork.Repository<Message, int>().GetAll();
           
            query = query.OrderByDescending(x => x.MessageSent);

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.Username && x.RecipientDeleted == false),
                "Outbox" => query.Where(x => x.Sender.UserName == messageParams.Username && x.SenderDeleted == false),
                _ => query.Where(x => x.Recipient.UserName == messageParams.Username && x.DateRead == null && x.RecipientDeleted == false)
            };

            var mappedQuery = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            var pagedList = await PagedList<MessageDto>.CreateAsync(
                mappedQuery,
                messageParams.PageNumber,
                messageParams.PageSize
            );

            return new PaginatedResponse<MessageDto>(pagedList);
        }

        public async Task<BaseResponse<IEnumerable<MessageDto>>> GetMessagesThread(string currentUsername, string receiverUsername)
        {
            // Retrieve the messages based on the filter conditions
            var messages = await _unitOfWork.Repository<Message, int>()
                .GetAllAsync(
                    filter: x => (x.Recipient.UserName == currentUsername
                                  && x.RecipientDeleted == false
                                  && x.Sender.UserName == receiverUsername)
                                 || (x.Recipient.UserName == receiverUsername
                                     && x.SenderDeleted == false
                                     && x.Sender.UserName == currentUsername),
                    orderBy: q => q.OrderBy(x => x.MessageSent)
                );

            if (messages == null || !messages.Any())
            {
                return new BaseResponse<IEnumerable<MessageDto>>(
                    statusCode: StatusCodes.Status404NotFound,
                    success: false,
                    message: "No messages found for this conversation."
                );
            }

            // Mark unread messages as read
            var unreadMessages = messages.Where(x => x.DateRead == null && x.Recipient.UserName == currentUsername);

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
                await _unitOfWork.SaveAsync();
            }

            // Map messages to DTOs
            var messageDtos = _mapper.Map<IEnumerable<MessageDto>>(messages);

            return new BaseResponse<IEnumerable<MessageDto>>(
                statusCode: StatusCodes.Status200OK,
                success: true,
                data: messageDtos
            );
        }



        //public async Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername, string receiverUsername)
        //{
        //    var messages = await _unitOfWork.Repository<Message, int>()
        //        .GetAllAsync(
        //            filter: x => (x.Recipient.UserName == currentUsername
        //                          && x.RecipientDeleted == false
        //                          && x.Sender.UserName == receiverUsername)
        //                         || (x.Recipient.UserName == receiverUsername
        //                             && x.SenderDeleted == false
        //                             && x.Sender.UserName == currentUsername),

        //            orderBy: q => q.OrderBy(x => x.MessageSent));

        //    var unreadMessages = messages.Where(x => x.DateRead == null && x.Recipient.UserName == currentUsername);

        //    if (unreadMessages.Count() != 0)
        //    {
        //        foreach (var message in unreadMessages)
        //        {
        //            message.DateRead = DateTime.UtcNow;
        //        }
        //        await _unitOfWork.SaveAsync();
        //    }

        //    return _mapper.Map<IEnumerable<MessageDto>>(messages);
        //}

        public async Task AddGroup(Group group)
        {
            await _unitOfWork.Repository<Group, string>().AddAsync(group);
        }

        public void RemoveConnection(Connection connection)
        {
            _unitOfWork.Repository<Connection, string>().Delete(connection);
        }

        public async Task<Connection?> GetConnection(string connectionId)
        {
            return await _unitOfWork.Repository<Connection, string>().GetAsync(connectionId);
        }

        public async Task<Group?> GetMessageGroup(string groupName)
        {
            return await _unitOfWork.Repository<Group, string>()
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<Group?> GetGroupForConnections(string connectionID)
        {
            return await _unitOfWork.Repository<Group, string>()
                .GetAll()
                .Include(c => c.Connections)
                .Where(x => x.Connections.Any(c => c.ConnectionId == connectionID))
                .FirstOrDefaultAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _unitOfWork.SaveAsync() > 0;
        }
    }
}

//public async Task<PaginatedResponse<MessageDto>> GetMessageForUser(MessageParams messageParams)
//{
//    // Get filtered and ordered list
//    var messages = await _unitOfWork.Repository<Message, int>()
//        .GetAllAsync(
//            orderBy: q => q.OrderByDescending(x => x.MessageSent),
//            filter: messageParams.Container switch
//            {
//                "Inbox" => x => x.Recipient.UserName == messageParams.Username,
//                "Outbox" => x => x.Sender.UserName == messageParams.Username,
//                _ => x => x.Recipient.UserName == messageParams.Username && x.DateRead == null
//            });

//    // Manual in-memory pagination
//    var count = messages.Count;
//    var items = messages
//        .Skip((messageParams.PageNumber - 1) * messageParams.PageSize)
//        .Take(messageParams.PageSize)
//        .ToList();

//    // Project to DTO
//    var mappedItems = _mapper.Map<List<MessageDto>>(items);

//    var pagedList = new PagedList<MessageDto>(mappedItems, count,
//        messageParams.PageNumber, messageParams.PageSize);

//    return new PaginatedResponse<MessageDto>(pagedList);
//}