using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MatchMaker.Service.Abstracts;
using MediatR;

namespace MatchMaker.Features.Authentication.Commands.ResetPassword
{
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, BaseResponse<string>>
    {
        private readonly IAuthService _authService;

        public ResetPasswordHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<BaseResponse<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.ResetPasswordAsync(request);

            if (result == "Invalid or expired OTP." || result == "Failed to reset password.")
            {
                return new BaseResponse<string>(400, false, result);
            }

            return new BaseResponse<string>(200, true, result);

        }
    }
}
