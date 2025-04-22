using MatchMaker.Core.Helper;
using MatchMaker.Service.Abstracts;
using MediatR;

namespace MatchMaker.Features.Authentication.Commands.SendOtp
{
    public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, BaseResponse<string>>
    {
        private readonly IAuthService _authService;

        public SendOtpCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<BaseResponse<string>> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {
            var existing = await _authService.CheckExistingUserByEmailAsync(request.Email);
            if (!existing)
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "This Email is not registered."
                };
            }
            var result = await _authService.SendOtpAsync(request.Email);
            if (result == "OTP has been sent to your email")
            {
                return new BaseResponse<string>(200, true, result);

            }
            return new BaseResponse<string>(400, false, result);


        }
    }
}
