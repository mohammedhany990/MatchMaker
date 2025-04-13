using AutoMapper.QueryableExtensions;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using MatchMaker.ExtensionMethods;
using MatchMaker.Infrastructure.Interfaces;
using MatchMaker.Service.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MatchMaker.Controllers
{
    [Authorize]
    public class UsersController : ApiBaseController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("all-users")]
        public async Task<ActionResult<PaginatedResponse<MemberDto>>> GetAllUsers([FromQuery] UserParams userParams)
        {
            userParams.CurrentUsername = User.GetEmail().Split("@")[0];
            var result = await _userService.GetAllUsersAsync(userParams, userParams.CurrentUsername);

            // Access the PagedList from the PaginatedResponse
            Response.AddPaginationHeader(result);
            return Ok(result);
        }

        [HttpGet("user-by-email")]
        public async Task<ActionResult<BaseResponse<MemberDto>>> GetUserByEmail(string email)
        {
            var result = await _userService.GetUserByEmailAsync(email);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<BaseResponse<PhotoDto>>> AddPhoto(IFormFile file)
        {
            var result = await _userService.AddPhotoAsync(file, User.GetEmail());

            if (result.StatusCode == StatusCodes.Status201Created)
            {
                return CreatedAtAction(
                    nameof(GetUserByEmail),
                    new { email = User.GetEmail() },
                    result);
            }

            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("set-main-photo/{photoId:int}")]
        [Authorize]
        public async Task<ActionResult<BaseResponse<string>>> SetMainPhoto(int photoId)
        {
            var result = await _userService.SetMainPhotoAsync(photoId, User.GetEmail());
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("delete-photo/{photoId:int}")]
        public async Task<ActionResult<BaseResponse<string>>> DeletePhoto(int photoId)
        {
            var result = await _userService.DeletePhotoAsync(photoId, User.GetEmail());
            return StatusCode(result.StatusCode, result);
        }
    }
}