using MatchMaker.Service.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Infrastructure.Interfaces;

namespace MatchMaker.Service.Implementations
{
    class LikeService : ILikeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LikeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<UserLike?> GetUserLike(string sourceUserId, int targetUserId)
        {
            return await _unitOfWork.Repository<UserLike, string>()
                .GetAsync(x=> x.SourceUserId == sourceUserId && x.TargetUserId == targetUserId)
        }

        public Task<IEnumerable<MemberDto>> GetUserLikes(string predicate, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetCurrentUserLikeIds(string currentUserId)
        {
            var ans = await _unitOfWork.Repository<UserLike, string>()
                .GetAllAsync(x => x.SourceUserId == currentUserId);
            
            var result = ans.Select(x => x.TargetUserId.ToString()).ToList();
            return result;
        }

        public void DeleteLike(UserLike like)
        {
            _unitOfWork.Repository<UserLike, string>().Delete(like);
        }

        public async Task AddLike(UserLike like)
        {
           await _unitOfWork.Repository<UserLike, string>().AddAsync(like);
        }
    }
}
