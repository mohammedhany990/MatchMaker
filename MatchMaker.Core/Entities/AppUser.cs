using MatchMaker.Core.ExtensionMethods;
using Microsoft.AspNetCore.Identity;

namespace MatchMaker.Core.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string KnownAs { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string Gender { get; set; }
        public string? Introduction { get; set; }
        public string? Interests { get; set; }
        public string? LookingFor { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        public List<Photo> Photos { get; set; } = new List<Photo>();

        public List<UserLike> LikedByUsers { get; set; } = new List<UserLike>();

        public List<UserLike> LikedUsers { get; set; } = new List<UserLike>();

        public List<Message> MessagesSent { get; set; } = new List<Message>();
        public List<Message> MessagesReceived { get; set; } = new List<Message>();

        public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();

        public List<RefreshToken>? RefreshTokens { get; set; } = new List<RefreshToken>();

        public int GetAge()
        {
            return DateOfBirth.CalculateAge();
        }
    }
}
