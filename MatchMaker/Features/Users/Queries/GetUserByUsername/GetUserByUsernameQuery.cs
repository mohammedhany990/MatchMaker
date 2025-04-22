using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Users.Queries.GetUserByUsername
{
    public class GetUserByUsernameQuery : IRequest<BaseResponse<MemberDto>>
    {
        public string Username { get; }

        public GetUserByUsernameQuery(string username)
        {
            Username = username;
        }
    }

}
