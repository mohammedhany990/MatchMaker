using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using MatchMaker.Infrastructure.Identity;
using MatchMaker.Service.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MatchMaker.Controllers
{
    
    public class AccountsController : ApiBaseController
    {
        private readonly IAuthService _authService;


        public AccountsController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<BaseResponse<string>>> Register(RegisterCommand command)
        {
            var res = await _authService.RegisterAsync(command);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<BaseResponse<UserResponse>>> Login(string email, string password)
        {
            var res = await _authService.LoginAsync(email, password);
            if (res is null)
            {
                return Ok("Null");
            }

            return Ok(res);
        }
    }
}
