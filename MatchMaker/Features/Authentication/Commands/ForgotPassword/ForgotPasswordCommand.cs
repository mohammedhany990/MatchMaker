using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Authentication.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<BaseResponse<string>>
    {
        public string Email { get; set; }
    }
}
