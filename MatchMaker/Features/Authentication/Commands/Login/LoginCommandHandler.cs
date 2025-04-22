using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MatchMaker.Service.Abstracts;
using MediatR;

namespace MatchMaker.Features.Authentication.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, BaseResponse<UserResponse>>
    {
        private readonly IAuthService _authService;

        public LoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<BaseResponse<UserResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _authService.GetUserByEmailAsync(request.Email); // assuming you have this method

            if (user == null)
            {
                return new BaseResponse<UserResponse>(404, false, "There is no account for this user.");
            }

            if (!await _authService.IsEmailConfirmedAsync(user.Email))
            {
                return new BaseResponse<UserResponse>(400, false, "Email is not confirmed.");
            }

            if (!await _authService.IsPasswordValid(user, request.Password))
            {
                return new BaseResponse<UserResponse>(400, false, "Invalid password.");
            }

            var result = await _authService.LoginAsync(request);

            if (result != null)
            {
                return new BaseResponse<UserResponse>(200, true, result, "Login successful.");
            }

            return new BaseResponse<UserResponse>(401, false, "Invalid email or password.");
        }

    }
}
