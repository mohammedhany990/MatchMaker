using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Users.Commands.DeletePhoto
{
    public class DeletePhotoCommand : IRequest<BaseResponse<string>>
    {
        public int PhotoId { get; set; }
        public string UserEmail { get; set; }
        public DeletePhotoCommand(int photoId, string userEmail)
        {
            PhotoId = photoId;
            UserEmail = userEmail;
        }
    }

}
