using System.Text.Json;
using MatchMaker.Core.Helper;

namespace MatchMaker.ExtensionMethods
{
    //public static class HttpExtension
    //{
    //    public static void AddPaginationHeader<T>(this HttpResponse response, PagedList<T> data)
    //    {
    //        var paginationHeader = new PaginationHeader(data.CurrentPage, data.PageSize, data.PageSize, data.TotalCount);

    //        var jsonOptions = new JsonSerializerOptions
    //        {
    //            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    //        };
    //        response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, jsonOptions));

    //        response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
    //    }
    //}

    public static class HttpExtensions
    {
        public static void AddPaginationHeader<T>(this HttpResponse response, PaginatedResponse<T> data)
        {
            var paginationHeader = new PaginationHeader(
                data.CurrentPage,
                data.PageSize,
                data.TotalPages,
                data.TotalCount);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, jsonOptions));
            response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
