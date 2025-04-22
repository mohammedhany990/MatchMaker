using AutoMapper;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using MatchMaker.Infrastructure.Interfaces;
using MatchMaker.Service.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MatchMaker.Service.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailSettings _emailSettings;
        private readonly IDatabase _redis;
        public AuthService(IConfiguration configuration,
            UserManager<AppUser> userManager,
            IConnectionMultiplexer redis,
            SignInManager<AppUser> signInManager,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IEmailSettings emailSettings
           )
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _emailSettings = emailSettings;
            _redis = redis.GetDatabase();

        }


        public async Task<BaseResponse<string>> RegisterAsync(RegisterCommand command)
        {

            var user = _mapper.Map<AppUser>(command);

            user.UserName = command.Email.Split('@')[0].ToLower();

            var result = await _userManager.CreateAsync(user, command.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return new BaseResponse<string>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Errors",
                    Errors = errors
                };
            }

            var isSent = await SendOtpAsync(user.Email);
            if (isSent != "OTP has been sent to your email")
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = isSent
                };
            }

            return new BaseResponse<string>
            {
                Success = true,
                StatusCode = 200,
                Message = "User Created Successfully, Please verify the OTP sent to your email."
            };
        }

        public async Task<UserResponse> LoginAsync(LoginCommand command)
        {
            var user = await _userManager.Users
                .Include(u => u.Photos)
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email == command.Email);

            if (user is null)
            {
                return null;
            }



            var jwtToken = await CreateJwtSecurityTokenAsync(user);

            var userResponse = new UserResponse
            {
                Username = user.UserName,
                Email = user.Email,
                KnownAs = user.KnownAs,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                ExpiresOn = jwtToken.ValidTo,
                PictureUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url ?? ""
            };

            var activeRefreshToken = user.RefreshTokens?.FirstOrDefault(rt => rt.IsActive);

            if (activeRefreshToken is not null)
            {
                userResponse.RefreshToken = activeRefreshToken.Token;
                userResponse.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var newRefreshToken = GenerateRefreshToken();
                user.RefreshTokens?.Add(newRefreshToken);
                await _userManager.UpdateAsync(user);

                userResponse.RefreshToken = newRefreshToken.Token;
                userResponse.RefreshTokenExpiration = newRefreshToken.ExpiresOn;
            }

            SetRefreshTokenInCookie(
                userResponse.RefreshToken,
                userResponse.RefreshTokenExpiration ?? DateTime.UtcNow.AddDays(10)
            );

            return userResponse;
        }

        public async Task<JwtSecurityToken> CreateJwtSecurityTokenAsync(AppUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtSettings = _configuration.GetSection("JWT");
            if (jwtSettings == null)
            {
                throw new InvalidOperationException("JWT settings are not configured.");
            }
            var key = jwtSettings["Key"];
            var issuer = jwtSettings["ValidIssuer"];
            var audience = jwtSettings["ValidAudience"];
            var durationInDays = double.TryParse(jwtSettings["DurationInDays"], out var duration) ? duration : 3;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(duration),
                signingCredentials: credentials
            );
        }

        public RefreshToken GenerateRefreshToken()
        {
            var randomBytes = new byte[32];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return new RefreshToken
            {
                Token = WebEncoders.Base64UrlEncode(randomBytes),
                CreateOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(10)
            };
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expiresOn)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expiresOn.ToLocalTime()
            };

            context.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        public async Task<bool> DeleteAccountAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return false;

            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> IsEmailConfirmedAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user!.EmailConfirmed;
        }

        public async Task<bool> IsUserExistsByIdAsync(int userId)
        {
            return await _userManager.Users.AnyAsync(i => i.Id == userId);
        }

        public async Task<bool> CheckExistingUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user is not null;
        }

        public async Task<bool> IsPasswordValid(AppUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<AppUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.Users
                .Include(u => u.Photos)
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email == email);
        }


        #region Password Manager

        public async Task<string> ResetPasswordAsync(ResetPasswordCommand request)
        {
            var storedOtp = await _redis.StringGetAsync(request.Email);
            if (string.IsNullOrEmpty(storedOtp) || storedOtp != request.Otp)
            {
                return "Invalid or expired OTP.";
            }
            var user = await _userManager.FindByEmailAsync(request.Email);

            var result = await _userManager.ResetPasswordAsync(user,
                await _userManager.GeneratePasswordResetTokenAsync(user),
                request.NewPassword);

            if (!result.Succeeded)
            {

                return "Failed to reset password.";
            }

            await _redis.KeyDeleteAsync(request.Email);
            return "Password has been reset successfully.";

        }


        public async Task<string> ChangePasswordAsync(ChangePasswordCommand request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            var passwordCheck = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if (!passwordCheck)
            {
                return "Current password is incorrect.";
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                return "Failed to change password.";
            }

            return "Password has been changed successfully.";
        }

        #endregion

        #region Otp Manager
        private string GenerateOtp()
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[4];
            rng.GetBytes(bytes);

            // Convert bytes to an integer and ensure a 6-digit OTP
            int otp = (int)(BitConverter.ToUInt32(bytes, 0) % 900000) + 100000;

            return otp.ToString();
        }

        private async Task StoreOtpAsync(string email, string otp)
        {
            // Remove existing OTP if it exists
            await _redis.KeyDeleteAsync(email);

            // Store the new OTP with a 5-minute expiration
            await _redis.StringSetAsync(email, otp, TimeSpan.FromMinutes(5));
        }

        public async Task<string> SendOtpAsync(string email)
        {
            try
            {
                var otp = GenerateOtp();

                await StoreOtpAsync(email, otp);
                var emailToSend = new Email()
                {
                    To = email,
                    Subject = "OTP Code",
                    Body = $"Your OTP code is {otp}",
                };

                _emailSettings.SendEmail(emailToSend);

                return "OTP has been sent to your email";
            }
            catch (Exception ex)
            {
                return $"Otb can't be sent. Error:{ex.Message}";
            }

        }

        public async Task<BaseResponse<UserResponse>> VerifyOtpAsync(string email, string otp)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new BaseResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "User not found."
                };
            }

            if (user.EmailConfirmed)
            {
                return new BaseResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Email is already verified."
                };
            }

            var storedOtp = await _redis.StringGetAsync(email);
            if (string.IsNullOrEmpty(storedOtp))
            {
                return new BaseResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "OTP has expired or does not exist."
                };
            }

            if (storedOtp != otp)
            {
                return new BaseResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Invalid OTP."
                };
            }

            // Mark email as verified
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            // Delete OTP after successful verification
            await _redis.KeyDeleteAsync(email);

            // Generate JWT Token
            var jwtSecurityToken = await CreateJwtSecurityTokenAsync(user);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            // Generate Refresh Token
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens?.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            // Set Refresh Token in Cookie
            SetRefreshTokenInCookie(refreshToken.Token, refreshToken.ExpiresOn);

            return new BaseResponse<UserResponse>
            {
                Success = true,
                StatusCode = 200,
                Message = "Email verified successfully. You are now logged in.",
                Data = new UserResponse
                {
                    Email = user.Email,
                    Username = user.UserName,
                    Token = accessToken,
                    RefreshToken = refreshToken.Token,
                    ExpiresOn = jwtSecurityToken.ValidTo,
                    RefreshTokenExpiration = refreshToken.ExpiresOn
                }
            };
        }

        #endregion
    }
}
