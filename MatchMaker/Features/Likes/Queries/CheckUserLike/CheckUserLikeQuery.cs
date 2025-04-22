using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Likes.Queries.CheckUserLike
{
    public class CheckUserLikeQuery : IRequest<BaseResponse<bool>>
    {
        public int SourceUserId { get; set; }
        public int TargetUserId { get; set; }

        public CheckUserLikeQuery(int sourceUserId, int targetUserId)
        {
            SourceUserId = sourceUserId;
            TargetUserId = targetUserId;
        }
    }
}
