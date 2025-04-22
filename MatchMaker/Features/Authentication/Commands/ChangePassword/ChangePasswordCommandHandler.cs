using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MatchMaker.Service.Abstracts;
using MediatR;

namespace MatchMaker.Features.Authentication.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, BaseResponse<string>>
    {
        private readonly IAuthService _authService;

        public ChangePasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<BaseResponse<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var existingEmail = await _authService.CheckExistingUserByEmailAsync(request.Email);
            if (!existingEmail)
            {
                return new BaseResponse<string>(404, false, "This Email is not registered.");
            }

            var result = await _authService.ChangePasswordAsync(request);

            return result == "Password has been changed successfully."
                ? new BaseResponse<string>(200, true, result)
                : new BaseResponse<string>(404, false, result);

        }
    }
}
