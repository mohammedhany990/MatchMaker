using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MatchMaker.ExtensionMethods;
using MatchMaker.Service.Abstracts;
using MediatR;

namespace MatchMaker.Features.Likes.Queries.GetUserLikes
{
    public class GetUserLikesHandler : IRequestHandler<GetUserLikesQuery, PaginatedResponse<MemberDto>>
    {
        private readonly ILikeService _likeService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetUserLikesHandler(ILikeService likeService, IHttpContextAccessor httpContextAccessor)
        {
            _likeService = likeService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginatedResponse<MemberDto>> Handle(GetUserLikesQuery request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetId() ?? 0;
            if (userId == 0)
            {
                throw new UnauthorizedAccessException("User not found");
            }
            request.CurrentUserId = userId;
            return await _likeService.GetUserLikes(request);
        }
    }

}
