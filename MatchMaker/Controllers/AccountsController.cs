using Asp.Versioning;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MatchMaker.Features.Authentication.Commands.Delete;
using MatchMaker.Features.Authentication.Commands.ForgotPassword;
using MatchMaker.Features.Authentication.Commands.SendOtp;
using MatchMaker.Features.Authentication.Commands.VerifyOtp;
using MatchMaker.Service.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchMaker.Controllers
{

    public class AccountsController : ApiBaseController
    {
        private readonly IMediator _mediator;


        public AccountsController( IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<BaseResponse<UserResponse>>> Register(RegisterCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<BaseResponse<UserResponse>>> Login(LoginCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }


        [Authorize]
        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteAccount()
        {
            var response = await _mediator.Send(new DeleteCommand());
            return Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpPost("send-otp")]
        public async Task<ActionResult> SndOtp(SendOtpCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpPost("verify-otp")]
        public async Task<ActionResult> VerifySignupOtp(VerifyOtpCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult<BaseResponse<string>>> ChangePassword(ChangePasswordCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpPost("forgot-password")]
        public async Task<ActionResult<BaseResponse<string>>> ForgotPassword(ForgotPasswordCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpPost("reset-password")]
        public async Task<ActionResult<BaseResponse<string>>> ResetPasswordAsync(ResetPasswordCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
