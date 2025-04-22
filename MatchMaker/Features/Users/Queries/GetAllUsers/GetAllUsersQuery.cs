using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<PaginatedResponse<MemberDto>>
    {
        public UserParams UserParams { get; set; }
      

        public GetAllUsersQuery(UserParams userParams)
        {
            UserParams = userParams;
        }
    }
}
