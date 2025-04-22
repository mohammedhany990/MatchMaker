using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Likes.Commands.RemoveLike
{
    public class RemoveLikeCommand : IRequest<BaseResponse<bool>>
    {
        public int SourceUserId { get; set; }
        public int TargetUserId { get; set; }
        public RemoveLikeCommand(int sourceUserId, int targetUserId)
        {
            SourceUserId = sourceUserId;
            TargetUserId = targetUserId;
        }
    }
}
