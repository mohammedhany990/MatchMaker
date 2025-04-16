namespace MatchMaker.Core.Helper
{
    public class LikesParams : PaginationParams
    {
        public string UserId { get; set; }
        public string Predicate { get; set; } = "liked";
    }
}
