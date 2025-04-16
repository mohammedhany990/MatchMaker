namespace MatchMaker.Core.Entities
{
    public class UserLike
    {
        public AppUser SourceUser { get; set; }
        public string SourceUserId { get; set; }

        public AppUser TargetUser { get; set; }
        public string TargetUserId { get; set; }
    }
}
