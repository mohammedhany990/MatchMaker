using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using MatchMaker.Infrastructure.Identity;
using MatchMaker.Service.Abstracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using MatchMaker.Infrastructure.Interfaces;

namespace MatchMaker.Service.Implementations
{
   public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDatabase _redis;
        public AuthService(IConfiguration configuration,
            UserManager<AppUser> userManager,
            IConnectionMultiplexer redis,
            SignInManager<AppUser> signInManager,
            IUnitOfWork unitOfWork,
            IMapper mapper
           )
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redis = redis.GetDatabase();

        }

        
        public async Task<BaseResponse<UserResponse>> RegisterAsync(RegisterCommand command)
        {
            //var user = new AppUser
            //{
            //    Email = command.Email,
            //    UserName = command.Email.Split('@')[0],
            //    City = command.City,
            //    Country = command.Country,
            //    Gender = command.Gender,
            //    //EmailConfirmed = false, // Not verified yet
            //};
            var user = _mapper.Map<AppUser>(command);
            user.UserName = command.Email.Split('@')[0];
            var result = await _userManager.CreateAsync(user, command.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return new BaseResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Errors",
                    Errors = errors
                };
            }

            //var isSent = await SendOtpAsync(user.Email); // Send OTP for verification

            //if (isSent != "OTP has been sent to your email")
            //{
            //    return new BaseResponse<string>
            //    {
            //        Success = false,
            //        StatusCode = 400,
            //        Message = isSent
            //    };
            //}
            var jwtSecurityToken = await CreateJwtSecurityTokenAsync(user);
            return new BaseResponse<UserResponse>
            {
                Data = new UserResponse
                {
                    Email = user.Email,
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    Username = user.UserName,
                    KnownAs = user.KnownAs,

                },
                Success = true,
                StatusCode = 200,
                Message = "User Created Successfully, Please verify the OTP sent to your email."
            };
        }


        public async Task<UserResponse> LoginAsync(string email, string password)
        {

            var user = await _userManager.Users
                .AsNoTracking()
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.Email == email);

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
                return null;

            var jwtSecurityToken = await CreateJwtSecurityTokenAsync(user);

            var returnedUser = new UserResponse
            {
                Username = user.UserName,
                Email = user.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                PictureUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url
                //ExpiresOn = jwtSecurityToken.ValidTo
            };

            //var activeToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
            //if (activeToken != null)
            //{
            //    returnedUser.RefreshToken = activeToken.Token;
            //    returnedUser.RefreshTokenExpiration = activeToken.ExpiresOn;
            //}
            //else
            //{
            //    var refreshToken = GenerateRefreshToken();
            //    returnedUser.RefreshToken = refreshToken.Token;
            //    returnedUser.RefreshTokenExpiration = refreshToken.ExpiresOn;

            //    user.RefreshTokens.Add(refreshToken);
            //    await _userManager.UpdateAsync(user);
            //}

            //if (!string.IsNullOrEmpty(returnedUser.RefreshToken))
            //    SetRefreshTokenInCookie(returnedUser.RefreshToken, returnedUser.RefreshTokenExpiration ?? DateTime.UtcNow.AddDays(10));


            return returnedUser;
        }

        public async Task<JwtSecurityToken> CreateJwtSecurityTokenAsync(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            var roles = await _userManager.GetRolesAsync(user);

            //claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["JWT:DurationInDays"])),
                claims: claims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            );

            return jwtSecurityToken;
        }


        public Task<bool> DeleteAccountAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsEmailConfirmedAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsUserExistsByIdAsync(string userId)
        {
            return await _userManager.Users.AnyAsync(i => i.Id == userId);
        }

        public Task<bool> CheckExistingUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}
