using AutoMapper;
using AutoMapper.QueryableExtensions;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using MatchMaker.Infrastructure.Interfaces;
using MatchMaker.Service.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace MatchMaker.Service.Implementations
{
   public class LikeService : ILikeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LikeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<UserLike?> GetUserLike(string sourceUserId, string targetUserId)
        {
            return await _unitOfWork.Repository<UserLike, string>()
                .GetAsync(x => x.SourceUserId == sourceUserId && x.TargetUserId == targetUserId);
        }

        public async Task<PaginatedResponse<MemberDto>> GetUserLikes(LikesParams likesParams)
        {
            var likes = await _unitOfWork.Repository<UserLike, string>().GetAllAsync();

            IQueryable<MemberDto> query;

            switch (likesParams.Predicate)
            {
                case "liked":
                    query = likes
                        .AsQueryable()
                        .Where(x => x.SourceUserId == likesParams.UserId)
                        .Select(like => like.TargetUser)
                        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);
                    break;

                case "likedBy":
                    query = likes
                        .AsQueryable()
                        .Where(x => x.TargetUserId == likesParams.UserId)
                        .Select(like => like.SourceUser)
                        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);

                    break;

                default:
                    var likeIds = await GetCurrentUserLikeIds(likesParams.UserId);
                    query = likes
                        .AsQueryable()
                        .Where(x => x.TargetUserId == likesParams.UserId && likeIds.Contains(x.SourceUserId))
                        .Select(like => like.SourceUser)
                        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);
                    break;
            }
           
            var result = PagedList<MemberDto>.Create(
                query,
                likesParams.PageNumber,
                likesParams.PageSize
            );
            return new PaginatedResponse<MemberDto>(result);
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

        public async Task<int> SaveAsync()
        {
            return await _unitOfWork.SaveAsync();
        }

    }
}
