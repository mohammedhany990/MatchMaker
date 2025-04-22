using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MatchMaker.ExtensionMethods;
using MatchMaker.Features.Likes.Commands.AddLike;
using MatchMaker.Features.Likes.Commands.RemoveLike;
using MatchMaker.Features.Likes.Queries.CheckUserLike;
using MatchMaker.Features.Likes.Queries.GetCurrentUserLikeIds;
using MatchMaker.Features.Likes.Queries.GetUserLikes;
using MatchMaker.Service.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchMaker.Controllers
{
    [Authorize]
    public class LikesController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public LikesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("add/{targetUserId}")]
        public async Task<ActionResult<BaseResponse<bool>>> AddLike(int targetUserId)
        {
            var result = await _mediator.Send(new AddLikeCommand(User.GetId(), targetUserId));
            return Ok( result);
        }

        [HttpDelete("remove/{targetUserId}")]
        public async Task<ActionResult<BaseResponse<bool>>> RemoveLike(int targetUserId)
        {
            var result = await _mediator.Send(new RemoveLikeCommand(User.GetId(), targetUserId));
            return Ok(result);
        }

        [HttpGet("list")]
        public async Task<ActionResult<BaseResponse<IEnumerable<int>>>> GetCurrentUserLikeIds()
        {
            var result = await _mediator.Send(new GetCurrentUserLikeIdsQuery(User.GetId()));
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<MemberDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            var query = new GetUserLikesQuery
            {
                Predicate = likesParams.Predicate,
                PageNumber = likesParams.PageNumber,
                PageSize = likesParams.PageSize
            };

            var result = await _mediator.Send(query);

            Response.AddPaginationHeader(result);
            return Ok(result);
        }
        [HttpGet("check-like")]
        public async Task<ActionResult<BaseResponse<bool>>> CheckUserLike([FromQuery] int sourceUserId, [FromQuery] int targetUserId)
        {
            var result = await _mediator.Send(new CheckUserLikeQuery(sourceUserId, targetUserId));
            return Ok(result);
        }

    }
}
