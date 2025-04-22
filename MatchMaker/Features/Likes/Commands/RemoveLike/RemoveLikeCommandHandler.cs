using MatchMaker.Core.Helper;
using MatchMaker.Service.Abstracts;
using MediatR;

namespace MatchMaker.Features.Likes.Commands.RemoveLike
{
    public class RemoveLikeCommandHandler : IRequestHandler<RemoveLikeCommand, BaseResponse<bool>>
    {
        private readonly ILikeService _likeService;

        public RemoveLikeCommandHandler(ILikeService likeService)
        {
            _likeService = likeService;
        }

        public async Task<BaseResponse<bool>> Handle(RemoveLikeCommand request, CancellationToken cancellationToken)
        {
            var like = await _likeService.GetUserLike(request.SourceUserId, request.TargetUserId);
            if (like == null)
            {
                return new BaseResponse<bool>(404, false, "Like not found.");
            }

            _likeService.DeleteLike(like);
            await _likeService.SaveAsync();

            return new BaseResponse<bool>(200, true, false, "Like removed successfully.");
        }
    }
}
