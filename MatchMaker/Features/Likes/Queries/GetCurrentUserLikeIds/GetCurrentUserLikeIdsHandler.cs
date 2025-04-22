using MatchMaker.Core.Helper;
using MatchMaker.Service.Abstracts;
using MediatR;

namespace MatchMaker.Features.Likes.Queries.GetCurrentUserLikeIds
{
    public class GetCurrentUserLikeIdsHandler : IRequestHandler<GetCurrentUserLikeIdsQuery, BaseResponse<IEnumerable<int>>>
    {
        private readonly ILikeService _likeService;

        public GetCurrentUserLikeIdsHandler(ILikeService likeService)
        {
            _likeService = likeService;
        }

        public async Task<BaseResponse<IEnumerable<int>>> Handle(GetCurrentUserLikeIdsQuery request, CancellationToken cancellationToken)
        {
            var ids = await _likeService.GetCurrentUserLikeIds(request.UserId);

            return new BaseResponse<IEnumerable<int>>(200, true, ids.ToList().Count, ids, "Retrieved successfully");
        }
    }

}
