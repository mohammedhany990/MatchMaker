using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Core.DTOs
{
    public class ResetPasswordCommand : IRequest<BaseResponse<string>>
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string Otp { get; set; }
    }
}
