using MatchMaker.Core.Helper;
using MatchMaker.ExtensionMethods;
using MatchMaker.Service.Abstracts;
using MediatR;

namespace MatchMaker.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, BaseResponse<string>>
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateUserCommandHandler(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<BaseResponse<string>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetId() ?? 0;
            if (userId == null || userId == 0)
            {
                return new BaseResponse<string>(StatusCodes.Status401Unauthorized, false, "Unauthorized");
            }
            
            var user = await _userService.GetUserByIdAsync(userId);
            if (user is null)
            {
                return new BaseResponse<string>(StatusCodes.Status404NotFound, false, "User not found");
            }
            user.KnownAs = request.KnownAs ?? user.KnownAs;
            user.Gender = request.Gender ?? user.Gender;
            user.Introduction = request.Introduction ?? user.Introduction;
            user.Interests = request.Interests ?? user.Interests;
            user.LookingFor = request.LookingFor ?? user.LookingFor;
            user.City = request.City ?? user.City;
            user.Country = request.Country ?? user.Country;

            var result = await _userService.UpdateUserAsync(user);
            if (result == "User updated successfully")
            {
                return new BaseResponse<string>(StatusCodes.Status200OK, true, result);
            }

            return new BaseResponse<string>(StatusCodes.Status500InternalServerError, false, "Failed to update user");
        }
    }
}
