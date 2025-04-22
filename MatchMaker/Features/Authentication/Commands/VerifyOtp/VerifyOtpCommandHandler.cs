using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MatchMaker.Service.Abstracts;
using MediatR;

namespace MatchMaker.Features.Authentication.Commands.VerifyOtp
{
    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, BaseResponse<UserResponse>>
    {
        private readonly IAuthService _authService;

        public VerifyOtpCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<BaseResponse<UserResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            return await _authService.VerifyOtpAsync(request.Email, request.Otp);

        }
    }
}
