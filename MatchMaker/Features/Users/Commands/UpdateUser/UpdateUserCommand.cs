using MatchMaker.Core.DTOs;
using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<BaseResponse<string>>
    {
        public string? KnownAs { get; set; }
        public string? Gender { get; set; }
        public string? Introduction { get; set; }
        public string? Interests { get; set; }
        public string? LookingFor { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
