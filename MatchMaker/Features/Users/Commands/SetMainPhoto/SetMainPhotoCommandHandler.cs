using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using MatchMaker.Infrastructure.Interfaces;
using MediatR;

namespace MatchMaker.Features.Users.Commands.SetMainPhoto
{
    public class SetMainPhotoCommandHandler : IRequestHandler<SetMainPhotoCommand, BaseResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SetMainPhotoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<string>> Handle(SetMainPhotoCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Repository<AppUser, string>()
                .GetAsync(u => u.Email == request.UserEmail);

            if (user is null)
                return new BaseResponse<string>(
                    statusCode: StatusCodes.Status404NotFound,
                    success: false,
                    message: "User not found");

            var photo = await _unitOfWork.Repository<Photo, int>().GetAsync(request.PhotoId);

            if (photo is null)
                return new BaseResponse<string>(
                    statusCode: StatusCodes.Status404NotFound,
                    success: false,
                    message: "Photo not found");

            if (photo.AppUserId != user.Id)
                return new BaseResponse<string>(
                    statusCode: StatusCodes.Status403Forbidden,
                    success: false,
                    message: "You don't have permission to access this resource");

            if (photo.IsMain)
                return new BaseResponse<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    success: false,
                    message: "This is already the main photo");

            var currentMain = await _unitOfWork.Repository<Photo, int>()
                .FirstOrDefaultAsync(p => p.IsMain && p.AppUserId == user.Id);

            if (currentMain is not null)
            {
                currentMain.IsMain = false;
                _unitOfWork.Repository<Photo, int>().Update(currentMain);
            }

            photo.IsMain = true;
            _unitOfWork.Repository<Photo, int>().Update(photo);

            var saved = await _unitOfWork.SaveAsync() > 0;
            if (!saved)
                return new BaseResponse<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    success: false,
                    message: "Problem setting main photo");

            return new BaseResponse<string>(
                statusCode: StatusCodes.Status200OK,
                success: true,
                message: "Main photo updated successfully");
        }
    }

}
