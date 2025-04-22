using MatchMaker.Core.Helper;
using MatchMaker.Service.Abstracts;
using MediatR;

namespace MatchMaker.Features.Likes.Queries.CheckUserLike
{
    public class CheckUserLikeHandler : IRequestHandler<CheckUserLikeQuery, BaseResponse<bool>>
    {
        private readonly ILikeService _likeService;

        public CheckUserLikeHandler(ILikeService likeService)
        {
            _likeService = likeService;
        }

        public async Task<BaseResponse<bool>> Handle(CheckUserLikeQuery request, CancellationToken cancellationToken)
        {
            var like = await _likeService.GetUserLike(request.SourceUserId, request.TargetUserId);

            return new BaseResponse<bool>(
                statusCode: 200,
                success: true,
                data: like != null,
                message: like != null ? "User has liked" : "User has not liked"
            );
        }
    }

}
