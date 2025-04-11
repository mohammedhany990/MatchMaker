using MatchMaker.Core.Helper;
using System.Text.Json.Serialization;

namespace MatchMaker.Errors
{
    public class ExceptionResponse : BaseResponse<string>
    {
        [JsonPropertyOrder(8)]
        public string? Details { get; set; }
        public ExceptionResponse(int code, string? msg = null, string? details = null)
            : base(code, msg)
        {
            Details = details;
        }
    }
}
