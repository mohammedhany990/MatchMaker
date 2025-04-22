using MatchMaker.Core.Helper;
using MediatR;

namespace MatchMaker.Features.Authentication.Commands.Delete
{
    public class DeleteCommand : IRequest<BaseResponse<string>>
    {
    }
}
