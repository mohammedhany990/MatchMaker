using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;

namespace MatchMaker.Service.Abstracts
{
    public interface ILikeService
    {
        Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId);
        Task<PaginatedResponse<MemberDto>> GetUserLikes(LikesParams likesParams);
        Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId);
        void DeleteLike(UserLike like);
        Task AddLike(UserLike like);
        Task<int> SaveAsync();
    }
}
