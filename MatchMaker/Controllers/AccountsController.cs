using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MatchMaker.Service.Abstracts;
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
        public async Task<ActionResult<BaseResponse<UserResponse>>> Login([FromBody]LoginCommand command)
        {
            var result = await _authService.LoginAsync(command);
            return Ok(result);
        }
    }
}
