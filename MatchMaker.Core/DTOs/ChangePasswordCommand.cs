using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Core.DTOs
{
    public class ChangePasswordCommand : IRequest<BaseResponse<string>>
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
