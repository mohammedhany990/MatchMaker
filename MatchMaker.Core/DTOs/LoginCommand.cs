using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Core.DTOs
{
    public class LoginCommand : IRequest<BaseResponse<UserResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
