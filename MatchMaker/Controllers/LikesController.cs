using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using MatchMaker.ExtensionMethods;
using MatchMaker.Service.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchMaker.Controllers
{
    [Authorize]
    public class LikesController : ApiBaseController
    {
        private readonly ILikeService _likeService;

        public LikesController(ILikeService likeService)
        {
            _likeService = likeService;
        }


        [HttpPost("{targetUserId}")]
        public async Task<ActionResult> ToggleLike(int targetUserId)
        {
            var sourceUserId = User.GetId() ;
           
            if (sourceUserId == targetUserId)
            {
                return BadRequest("You cannot like yourself.");
            }

            var like = await _likeService.GetUserLike(sourceUserId, targetUserId);
            if (like != null)
            {
                _likeService.DeleteLike(like);
                await _likeService.SaveAsync();
                return Ok(new { liked = false });
            }

            var newLike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId
            };
            await _likeService.AddLike(newLike);
            await _likeService.SaveAsync();
            return Ok(new { liked = true });
        }


        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<string>>> GetCurrentUserLikeIds()
        {
            return Ok(await _likeService.GetCurrentUserLikeIds(User.GetId()));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetId();

            var users = await _likeService.GetUserLikes(likesParams);
            Response.AddPaginationHeader(users);
            return Ok(users);
        }



        [HttpGet("check-like")]
        public async Task<ActionResult<bool>> CheckUserLike([FromQuery] int sourceUserId, [FromQuery] int targetUserId)
        {
            var like = await _likeService.GetUserLike(sourceUserId, targetUserId);
            return Ok(like != null);
        }
    }

}
