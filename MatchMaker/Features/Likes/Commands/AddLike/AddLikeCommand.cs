using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Likes.Commands.AddLike
{
    public class AddLikeCommand : IRequest<BaseResponse<object>>
    {
        public int SourceUserId { get; set; }
        public int TargetUserId { get; set; }

        public AddLikeCommand(int sourceUserId, int targetUserId)
        {
            SourceUserId = sourceUserId;
            TargetUserId = targetUserId;
        }
    }

}
