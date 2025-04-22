using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using MatchMaker.Service.Abstracts;
using MediatR;

namespace MatchMaker.Features.Likes.Commands.AddLike
{
    public class AddLikeCommandHandler : IRequestHandler<AddLikeCommand, BaseResponse<object>>
    {
        private readonly ILikeService _likeService;

        public AddLikeCommandHandler(ILikeService likeService)
        {
            _likeService = likeService;
        }

        public async Task<BaseResponse<object>> Handle(AddLikeCommand request, CancellationToken cancellationToken)
        {
            var existingLike = await _likeService.GetUserLike(request.SourceUserId, request.TargetUserId);
            if (existingLike != null)
            {
                return new BaseResponse<object>(
                    statusCode: 400,
                    success: false,
                    message: "You already liked this user."
                );
            }

            var like = new UserLike
            {
                SourceUserId = request.SourceUserId,
                TargetUserId = request.TargetUserId
            };

            await _likeService.AddLike(like);
            var result = await _likeService.SaveAsync();

            if (result > 0)
            {
                return new BaseResponse<object>(
                    statusCode: 201,
                    success: true,
                    data: null,
                    message: "Like added successfully."
                );
            }

            return new BaseResponse<object>(
                statusCode: 500,
                success: false,
                message: "Failed to add like."
            );
        }
    }
}
