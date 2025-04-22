using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Likes.Queries.GetCurrentUserLikeIds
{
    public class GetCurrentUserLikeIdsQuery : IRequest<BaseResponse<IEnumerable<int>>>
    {
        public int UserId { get; set; }

        public GetCurrentUserLikeIdsQuery(int userId)
        {
            UserId = userId;
        }
    }
}
