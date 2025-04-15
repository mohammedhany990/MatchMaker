using AutoMapper;
using AutoMapper.QueryableExtensions;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using MatchMaker.Core.Specifications;
using MatchMaker.Infrastructure.Interfaces;
using MatchMaker.Service.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MatchMaker.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(
            UserManager<AppUser> userManager,
            IPhotoService photoService,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _photoService = photoService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResponse<MemberDto>> GetAllUsersAsync(
            UserParams userParams,
            string currentUsername)
        {
            
            var spec = new UserSpecification(userParams, currentUsername);
          
            var query = SpecificationEvaluator<AppUser>.GetQuery(
                _userManager.Users.AsQueryable(),
                spec
            );

            var users = await PagedList<MemberDto>.CreateAsync(
                query.AsNoTracking()
                    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider),
                userParams.PageNumber,
                userParams.PageSize
            );

            return new PaginatedResponse<MemberDto>(users);
        }

        public async Task<BaseResponse<MemberDto>> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.Users
                .AsNoTracking()
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return new BaseResponse<MemberDto>(
                    statusCode: StatusCodes.Status404NotFound,
                    success: false,
                    message: "User not found"
                );
            }

            var mappedUser = _mapper.Map<MemberDto>(user);
            return new BaseResponse<MemberDto>(
                statusCode: StatusCodes.Status200OK,
                success: true,
                data: mappedUser
            );
        }

        public async Task<BaseResponse<PhotoDto>> AddPhotoAsync(IFormFile file, string userEmail)
        {
            var user = await _unitOfWork.Repository<AppUser, string>()
                .GetAsync(x => x.Email == userEmail);

            if (user is null)
            {
                return new BaseResponse<PhotoDto>(
                    statusCode: StatusCodes.Status400BadRequest,
                    success: false,
                    message: "User not found"
                );
            }

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error is not null)
            {
                return new BaseResponse<PhotoDto>(
                    statusCode: StatusCodes.Status400BadRequest,
                    success: false,
                    message: result.Error.Message
                );
            }

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            user.Photos.Add(photo);
            var mappedPhoto = _mapper.Map<PhotoDto>(photo);

            if (await _unitOfWork.SaveAsync() > 0)
            {
                return new BaseResponse<PhotoDto>(
                    statusCode: StatusCodes.Status201Created,
                    success: true,
                    data: mappedPhoto,
                    message: "Photo added successfully"
                );
            }

            return new BaseResponse<PhotoDto>(
                statusCode: StatusCodes.Status400BadRequest,
                success: false,
                message: "Problem adding photo"
            );
        }

        public async Task<BaseResponse<string>> SetMainPhotoAsync(int photoId, string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                return new BaseResponse<string>(
                    statusCode: StatusCodes.Status401Unauthorized,
                    success: false,
                    message: "User not authenticated"
                );
            }

            var user = await _unitOfWork.Repository<AppUser, string>()
                .GetAsync(x => x.Email == userEmail);

            if (user is null)
            {
                return new BaseResponse<string>(
                    statusCode: StatusCodes.Status404NotFound,
                    success: false,
                    message: "User not found"
                );
            }

            var photo = await _unitOfWork.Repository<Photo, int>().GetAsync(photoId);
            if (photo is null)
            {
                return new BaseResponse<string>(
                    statusCode: StatusCodes.Status404NotFound,
                    success: false,
                    message: "Photo not found"
                );
            }

            if (photo.AppUserId != user.Id)
            {
                return new BaseResponse<string>(
                    statusCode: StatusCodes.Status403Forbidden,
                    success: false,
                    message: "You don't have permission to access this resource"
                );
            }

            if (photo.IsMain)
            {
                return new BaseResponse<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    success: false,
                    message: "This is already the main photo"
                );
            }

            var currentMain = await _unitOfWork.Repository<Photo, int>()
                .FirstOrDefaultAsync(x => x.IsMain && x.AppUserId == user.Id);

            if (currentMain is not null)
            {
                currentMain.IsMain = false;
                _unitOfWork.Repository<Photo, int>().Update(currentMain);
            }

            photo.IsMain = true;
            _unitOfWork.Repository<Photo, int>().Update(photo);

            var result = await _unitOfWork.SaveAsync() > 0;
            if (!result)
            {
                return new BaseResponse<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    success: false,
                    message: "Problem setting main photo"
                );
            }

            return new BaseResponse<string>(
                statusCode: StatusCodes.Status200OK,
                success: true,
                message: "Main photo updated successfully"
            );
        }

        public async Task<BaseResponse<string>> DeletePhotoAsync(int photoId, string userEmail)
        {
            var user = await _userManager.Users
                .AsNoTracking()
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.Email.ToUpper() == userEmail.ToUpper());

            if (user is null)
            {
                return new BaseResponse<string>(
                    statusCode: StatusCodes.Status404NotFound,
                    success: false,
                    message: "User not found"
                );
            }

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
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