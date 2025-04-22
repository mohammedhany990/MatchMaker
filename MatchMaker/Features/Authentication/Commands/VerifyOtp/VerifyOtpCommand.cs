using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Authentication.Commands.VerifyOtp
{
    public class VerifyOtpCommand : IRequest<BaseResponse<UserResponse>>
    {
        public string Email { get; set; }
        public string Otp { get; set; }

    }
}
