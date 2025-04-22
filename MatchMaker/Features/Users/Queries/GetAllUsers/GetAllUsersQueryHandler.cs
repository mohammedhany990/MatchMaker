using AutoMapper;
using AutoMapper.QueryableExtensions;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;
using MatchMaker.Core.Specifications;
using MatchMaker.ExtensionMethods;
using MatchMaker.Service.Abstracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MatchMaker.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PaginatedResponse<MemberDto>>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetAllUsersQueryHandler(IUserService userService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginatedResponse<MemberDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            //var email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var email = _httpContextAccessor.HttpContext.User.GetEmail();

            var user = await _userService.GetUserByEmailAsync(email);

            var members = await _userService.GetAllUsersAsync(request.UserParams, user.UserName);

            return members;
        }
    }
}
