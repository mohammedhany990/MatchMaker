using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using MatchMaker.Infrastructure.Interfaces;
using MatchMaker.Service.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MatchMaker.Features.Users.Commands.DeletePhoto
{
    public class DeletePhotoCommandHandler : IRequestHandler<DeletePhotoCommand, BaseResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;
        private readonly UserManager<AppUser> _userManager;

        public DeletePhotoCommandHandler(IUnitOfWork unitOfWork, IPhotoService photoService, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _photoService = photoService;
            _userManager = userManager;
        }

        public async Task<BaseResponse<string>> Handle(DeletePhotoCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .AsNoTracking()
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.Email.ToUpper() == request.UserEmail.ToUpper());

            if (user is null)
            {
                return new BaseResponse<string>(
                    statusCode: StatusCodes.Status404NotFound,
                    success: false,
                    message: "User not found"
                );
            }

            var photo = user.Photos.FirstOrDefault(x => x.Id == request.PhotoId);
            if (photo is null || photo.IsMain)
            {
                return new BaseResponse<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    success: false,
                    message: "You cannot delete this photo"
                );
            }

            if (photo.PublicId is not null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error is not null)
                {
                    return new BaseResponse<string>(
                        statusCode: StatusCodes.Status400BadRequest,
                        success: false,
                        message: result.Error.Message
                    );
                }
            }

            _unitOfWork.Repository<Photo, int>().Delete(photo);
            if (await _unitOfWork.SaveAsync() > 0)
            {
                return new BaseResponse<string>(
                    statusCode: StatusCodes.Status200OK,
                    success: true,
                    message: "Photo deleted successfully"
                );
            }

            return new BaseResponse<string>(
                statusCode: StatusCodes.Status400BadRequest,
                success: false,
                message: "Problem deleting photo"
            );
        }
    }

}
