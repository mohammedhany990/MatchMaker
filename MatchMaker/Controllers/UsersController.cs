using AutoMapper;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using MatchMaker.ExtensionMethods;
using MatchMaker.Infrastructure.Interfaces;
using MatchMaker.Service.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MatchMaker.Controllers
{
    [Authorize]
    public class UsersController : ApiBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(UserManager<AppUser> userManager, IPhotoService photoService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _photoService = photoService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("all-users")]
        public async Task<ActionResult<BaseResponse<List<MemberDto>>>> GetAllUsers()
        {
            var users = await _userManager.Users
                .AsNoTracking()
                .Include(u => u.Photos)
                .ToListAsync();

            var mappedUsers = _mapper.Map<List<AppUser>, List<MemberDto>>(users);

            return Ok(new BaseResponse<List<MemberDto>>(
                statusCode: StatusCodes.Status200OK,
                success: true,
                count: mappedUsers.Count,
                data: mappedUsers
            ));
        }

        [HttpGet("user-by-email")]
        public async Task<ActionResult<BaseResponse<MemberDto>>> GetUserByEmail(string email)
        {
            var user = await _userManager.Users
                .AsNoTracking()
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound(new BaseResponse<MemberDto>(
                    statusCode: StatusCodes.Status404NotFound,
                    success: false,
                    message: "User not found"
                ));
            }

            var mappedUser = _mapper.Map<AppUser, MemberDto>(user);

            return Ok(new BaseResponse<MemberDto>(
                statusCode: StatusCodes.Status200OK,
                success: true,
                data: mappedUser
            ));
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<BaseResponse<PhotoDto>>> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.Repository<AppUser, string>()
                .GetAsync(x => x.Email == User.GetEmail());

            if (user is null)
            {
                return BadRequest(new BaseResponse<PhotoDto>(
                    statusCode: StatusCodes.Status400BadRequest,
                    success: false,
                    message: "User not found"
                ));
            }

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error is not null)
            {
                return BadRequest(new BaseResponse<PhotoDto>(
                    statusCode: StatusCodes.Status400BadRequest,
                    success: false,
                    message: result.Error.Message
                ));
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
                return CreatedAtAction(
                    nameof(GetUserByEmail),
                    new { username = user.UserName, mappedPhoto },
                    new BaseResponse<PhotoDto>(
                        statusCode: StatusCodes.Status201Created,
                        success: true,
                        data: mappedPhoto,
                        message: "Photo added successfully"
                    ));
            }

            return BadRequest(new BaseResponse<PhotoDto>(
                statusCode: StatusCodes.Status400BadRequest,
                success: false,
                message: "Problem adding photo"
            ));
        }

        [HttpPut("set-main-photo/{photoId:int}")]
        [Authorize]
        public async Task<ActionResult<BaseResponse<string>>> SetMainPhoto(int photoId)
        {
            var userEmail = User.GetEmail();
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new BaseResponse<string>(
                    statusCode: StatusCodes.Status401Unauthorized,
                    success: false,
                    message: "User not authenticated"
                ));
            }

            var user = await _unitOfWork.Repository<AppUser, string>()
                .GetAsync(x => x.Email == userEmail);

            if (user is null)
            {
                return NotFound(new BaseResponse<string>(
                    statusCode: StatusCodes.Status404NotFound,
                    success: false,
                    message: "User not found"
                ));
            }

            var photo = await _unitOfWork.Repository<Photo, int>().GetAsync(photoId);
            if (photo is null)
            {
                return NotFound(new BaseResponse<string>(
                    statusCode: StatusCodes.Status404NotFound,
                    success: false,
                    message: "Photo not found"
                ));
            }

            if (photo.AppUserId != user.Id)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new BaseResponse<string>(
                    statusCode: StatusCodes.Status403Forbidden,
                    success: false,
                    message: "You don't have permission to access this resource"
                ));
            }

            if (photo.IsMain)
            {
                return BadRequest(new BaseResponse<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    success: false,
                    message: "This is already the main photo"
                ));
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
                return BadRequest(new BaseResponse<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    success: false,
                    message: "Problem setting main photo"
                ));
            }

            return Ok(new BaseResponse<string>(
                statusCode: StatusCodes.Status200OK,
                success: true,
                message: "Main photo updated successfully"
            ));
        }

        [HttpDelete("delete-photo/{photoId:int}")]
        public async Task<ActionResult<BaseResponse<string>>> DeletePhoto(int photoId)
        {
            var user = await _userManager.Users
                .AsNoTracking()
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.Email.ToUpper() == User.GetEmail().ToUpper());

            if (user is null)
            {
                return NotFound(new BaseResponse<string>(
                    statusCode: StatusCodes.Status404NotFound,
                    success: false,
                    message: "User not found"
                ));
            }

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            //var photo = await _unitOfWork.Repository<Photo, int>().GetAsync(photoId);
            if (photo is null || photo.IsMain)
            {
                return NotFound(new BaseResponse<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    success: false,
                    message: "You cannot delete this photo"
                ));
            }

            if (photo.PublicId is not null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error is not null)
                {
                    return BadRequest(new BaseResponse<string>(
                        statusCode: StatusCodes.Status400BadRequest,
                        success: false,
                        message: result.Error.Message
                    ));
                }
            }
            
            _unitOfWork.Repository<Photo, int>().Delete(photo);
            if (await _unitOfWork.SaveAsync() > 0)
            {
                return Ok(new BaseResponse<string>(
                    statusCode: StatusCodes.Status200OK,
                    success: true,
                    message: "Photo deleted successfully"
                ));
            }
            return BadRequest(new BaseResponse<string>(
                statusCode: StatusCodes.Status400BadRequest,
                success: false,
                message: "Problem deleting photo"
            ));

        }
    }
}