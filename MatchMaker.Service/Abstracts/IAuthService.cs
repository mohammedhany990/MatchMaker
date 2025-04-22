using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using System.IdentityModel.Tokens.Jwt;

namespace MatchMaker.Service.Abstracts
{
    public interface IAuthService
    {
        Task<BaseResponse<string>> RegisterAsync(RegisterCommand request);

        Task<UserResponse> LoginAsync(LoginCommand command);

        Task<bool> DeleteAccountAsync(string email);

        Task<bool> IsEmailConfirmedAsync(string email);

        Task<bool> IsUserExistsByIdAsync(int userId);

        Task<bool> CheckExistingUserByEmailAsync(string email);

        Task<AppUser?> GetUserByEmailAsync(string email);


        Task<JwtSecurityToken> CreateJwtSecurityTokenAsync(AppUser user);



        public Task<bool> IsPasswordValid(AppUser user, string password);

        //Task<UserDto> RefreshToken(HttpRequest request, HttpResponse response);
        //Task<BaseResponse<string>> RevokeToken(string? token, HttpRequest request);


        //Task<UserDto> RefreshTokenAsync(string refreshToken);
        //Task<bool> RevokeTokenAsync(string token);


        Task<string> ResetPasswordAsync(ResetPasswordCommand resetPasswordDto);
        Task<string> ChangePasswordAsync(ChangePasswordCommand request);

        Task<BaseResponse<UserResponse>> VerifyOtpAsync(string email, string otp);
        Task<string> SendOtpAsync(string email);
    }
}
