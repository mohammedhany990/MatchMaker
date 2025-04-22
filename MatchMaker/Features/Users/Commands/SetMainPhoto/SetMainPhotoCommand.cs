using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Users.Commands.SetMainPhoto
{
    public class SetMainPhotoCommand : IRequest<BaseResponse<string>>
    {
        public int PhotoId { get; set; }
        public string UserEmail { get; set; }

        public SetMainPhotoCommand(int photoId, string userEmail)
        {
            PhotoId = photoId;
            UserEmail = userEmail;
        }
    }

}
