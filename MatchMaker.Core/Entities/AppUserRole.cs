using Microsoft.AspNetCore.Identity;

namespace MatchMaker.Core.Entities
{
    public class AppUserRole : IdentityUserRole<int>
    {
        public AppUser User { get; set; } = null;
        public AppRole Role { get; set; } = null;

    }
}
