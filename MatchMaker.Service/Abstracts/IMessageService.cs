using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;

namespace MatchMaker.Service.Abstracts
{
   public interface IMessageService
   {
       Task AddMessage(Message message);
       Task<string> DeleteMessage(int messageId, int userId);

        Task<Message?> GetMessageByIdAsync(int id);
        Task<PaginatedResponse<MessageDto>> GetMessageForUser(MessageParams messageParams);
        
        Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername, string receiverUsername);

        Task AddGroup(Group group);
       
        void RemoveConnection(Connection connection);

        Task<Connection?> GetConnection(string connectionId);
        Task<Group?> GetMessageGroup(string groupName);

        Task<Group?> GetGroupForConnections(string connectionID);
        Task<bool> SaveAllAsync();
    }
}
