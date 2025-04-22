using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;

namespace MatchMaker.Service.Abstracts
{
    public interface IMessageService
    {
        Task AddMessage(Message message);
        Task<BaseResponse<string>> DeleteMessageAsync(int messageId, int userId);

        Task<Message?> GetMessageByIdAsync(int id);
        Task<PaginatedResponse<MessageDto>> GetMessageForUser(MessageParams messageParams);

        Task<BaseResponse<IEnumerable<MessageDto>>> GetMessagesThread(string currentUsername, string receiverUsername);

        Task AddGroup(Group group);

        void RemoveConnection(Connection connection);

        Task<Connection?> GetConnection(string connectionId);
        Task<Group?> GetMessageGroup(string groupName);

        Task<Group?> GetGroupForConnections(string connectionID);

        Task<bool> SaveAllAsync();
    }
}
