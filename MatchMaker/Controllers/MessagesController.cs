using AutoMapper;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using MatchMaker.ExtensionMethods;
using MatchMaker.Features.Messages.Commands;
using MatchMaker.Features.Messages.Commands.DeleteMessage;
using MatchMaker.Features.Messages.Queries.GetMessagesForUser;
using MatchMaker.Features.Messages.Queries.GetMessageThread;
using MatchMaker.Service.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MatchMaker.Controllers
{
    [Authorize]
    public class MessagesController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public MessagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("add-message")]
        public async Task<ActionResult<BaseResponse<MessageDto>>> CreateMessage([FromBody] CreateMessageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var result = await _mediator.Send(new GetMessagesForUserQuery{ MessageParams = messageParams });

            Response.AddPaginationHeader(result);

            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<BaseResponse<string>>> DeleteMessage(int id)
        {
            var result = await _mediator.Send(new DeleteMessageCommand(id, User.GetId()));
            return Ok(result);
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<MessageDto>>>> GetMessagesThread(string username)
        {
            var result = await _mediator.Send(new GetMessagesThreadQuery(User.GetUsername(), username));
            return Ok(result);
        }
    }
}
