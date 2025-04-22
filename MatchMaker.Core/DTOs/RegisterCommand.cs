using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Core.DTOs
{
    public class RegisterCommand : IRequest<BaseResponse<string>>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string? Gender { get; set; }
        public string? KnownAs { get; set; }
        public string? DateOfBirth { get; set; }

        public string? City { get; set; }
        public string? Country { get; set; }

    }
}
