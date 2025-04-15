using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;

namespace MatchMaker.Service.Abstracts
{
    public interface ILikeService
    {
        Task<UserLike?> GetUserLike(string sourceUserId, int targetUserId);
        Task<IEnumerable<MemberDto>> GetUserLikes(string predicate, string userId);
        Task<IEnumerable<string>> GetCurrentUserLikeIds(string currentUserId);
        void DeleteLike(UserLike like);
        Task AddLike(UserLike like);
    }
}
