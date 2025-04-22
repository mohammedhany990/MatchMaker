using FluentValidation;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MatchMaker.ExtensionMethods;
using MatchMaker.Features.Users.Commands.AddPhoto;
using MatchMaker.Features.Users.Commands.DeletePhoto;
using MatchMaker.Features.Users.Commands.SetMainPhoto;
using MatchMaker.Features.Users.Commands.UpdateUser;
using MatchMaker.Features.Users.Queries;
using MatchMaker.Features.Users.Queries.GetAllUsers;
using MatchMaker.Features.Users.Queries.GetUserByEmail;
using MatchMaker.Features.Users.Queries.GetUserByUsername;
using MatchMaker.Service.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchMaker.Controllers
{
    [Authorize]
    public class UsersController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all-users")]
        public async Task<ActionResult<PaginatedResponse<MemberDto>>> GetAllUsers([FromQuery] UserParams userParams)
        {
            var result = await _mediator.Send(new GetAllUsersQuery(userParams));
            Response.AddPaginationHeader(result);
            return Ok(result);
        }

        [Authorize(Roles = "Member")]
        [HttpGet("user-by-email")]
        public async Task<ActionResult<BaseResponse<MemberDto>>> GetUserByEmail(string email)
        {
            var result = await _mediator.Send(new GetUserByEmailQuery(email));
            return Ok(result);
        }

        [Authorize(Roles = "Member")]
        [HttpGet("user-by-username")]
        public async Task<ActionResult<BaseResponse<MemberDto>>> GetUserByUsername([FromQuery] string username)
        {
            var result = await _mediator.Send(new GetUserByUsernameQuery(username));
            return Ok(result);
        }

       
        [HttpPost("add-photo")]
        public async Task<ActionResult<BaseResponse<PhotoDto>>> AddPhoto(IFormFile file)
        {
            var result = await _mediator.Send(new AddPhotoCommand(file));
            return Ok(result);
        }

        [HttpPut("set-main-photo/{photoId:int}")]
        public async Task<ActionResult<BaseResponse<string>>> SetMainPhoto(int photoId)
        {
            try
            {
                var result = await _mediator.Send(new SetMainPhotoCommand(photoId, User.GetEmail()));

                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new BaseResponse<string>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Success = false,
                    Message = "Validation failed",
                    Errors = ex.Errors.Select(e => e.ErrorMessage)
                });
            }
        }

        [HttpDelete("delete-photo/{photoId:int}")]
        public async Task<ActionResult<BaseResponse<string>>> DeletePhoto(int photoId)
        {
            var result = await _mediator.Send(new DeletePhotoCommand(photoId, User.GetEmail()));
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<ActionResult<BaseResponse<string>>> UpdateUser([FromBody] UpdateUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}