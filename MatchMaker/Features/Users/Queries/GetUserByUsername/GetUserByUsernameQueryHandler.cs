using AutoMapper;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MatchMaker.Service.Abstracts;
using MediatR;

namespace MatchMaker.Features.Users.Queries.GetUserByUsername
{
    public class GetUserByUsernameQueryHandler : IRequestHandler<GetUserByUsernameQuery, BaseResponse<MemberDto>>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public GetUserByUsernameQueryHandler(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<BaseResponse<MemberDto>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByUsernameAsync(request.Username);

            if (user is null)
            {
                return new BaseResponse<MemberDto>(
                    statusCode: StatusCodes.Status404NotFound,
                    success: false,
                    message: "User not found"
                );
            }

            var dto = _mapper.Map<MemberDto>(user);

            return new BaseResponse<MemberDto>(
                statusCode: StatusCodes.Status200OK,
                success: true,
                data: dto
            );
        }
    }

}
