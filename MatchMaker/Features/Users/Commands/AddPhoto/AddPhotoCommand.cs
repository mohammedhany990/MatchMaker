using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Users.Commands.AddPhoto
{
    public class AddPhotoCommand : IRequest<BaseResponse<PhotoDto>>
    {
        public IFormFile File { get; }

        public AddPhotoCommand(IFormFile file)
        {
            File = file;
        }
    }

}
