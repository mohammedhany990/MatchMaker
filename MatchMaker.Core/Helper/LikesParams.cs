namespace MatchMaker.Core.Helper
{
    public class LikesParams : PaginationParams
    {
        public int UserId { get; set; }
        public string Predicate { get; set; } = "liked";
    }
}
