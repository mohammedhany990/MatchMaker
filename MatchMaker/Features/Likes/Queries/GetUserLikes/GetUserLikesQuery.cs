using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Likes.Queries.GetUserLikes
{
    public class GetUserLikesQuery : LikesParams, IRequest<PaginatedResponse<MemberDto>>
    {
        public int CurrentUserId { get; set; }  
    }
}
