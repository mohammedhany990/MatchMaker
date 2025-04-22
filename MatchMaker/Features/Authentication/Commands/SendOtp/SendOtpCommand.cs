using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Authentication.Commands.SendOtp
{
    public class SendOtpCommand : IRequest<BaseResponse<string>>
    {
        public string Email { get; set; }
    }
}
