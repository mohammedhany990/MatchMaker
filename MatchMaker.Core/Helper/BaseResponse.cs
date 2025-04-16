using System.Text.Json.Serialization;

namespace MatchMaker.Core.Helper
{
    public class BaseResponse<T>
    {
        [JsonPropertyOrder(1)]
        public int StatusCode { get; set; }

        [JsonPropertyOrder(2)]
        public string Message { get; set; }

        [JsonPropertyOrder(3)]
        public bool Success { get; set; }

        [JsonPropertyOrder(4)]
        public int Count { get; set; }

        [JsonPropertyOrder(5)]
        public T? Data { get; set; }

        [JsonPropertyOrder(6)]
        public IEnumerable<string> Errors { get; set; }


        public BaseResponse()
        {

        }

        public BaseResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        public BaseResponse(int statusCode, bool success, int count, T? data, string? message = null)
        {
            StatusCode = statusCode;
            Data = data;
            Count = count;
            Success = success;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        public BaseResponse(int statusCode, bool success, string? message = null)
        {
            StatusCode = statusCode;
            Success = success;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);

        }

        public BaseResponse(int statusCode, bool success, T? data, string? message = null)
        {
            StatusCode = statusCode;
            Success = success;
            Data = data;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);

        }

        public string? GetDefaultMessageForStatusCode(int code)
        {
            return code switch
            {
                200 => "OK",
                201 => "Created",
                204 => "No Content",
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                405 => "Method Not Allowed",
                408 => "Request Timeout",
                409 => "Conflict",
                429 => "Too Many Requests",
                500 => "Internal Server Error",
                502 => "Bad Gateway",
                503 => "Service Unavailable",
                504 => "Gateway Timeout",
                _ => null
            };
        }
    }
}
