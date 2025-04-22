using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Users.Queries.GetUserByEmail
{
    public class GetUserByEmailQuery : IRequest<BaseResponse<MemberDto>>
    {
        public string Email { get; set; }

        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }
    }

}
