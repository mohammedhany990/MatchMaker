using Microsoft.AspNetCore.Identity;

namespace MatchMaker.Core.Entities
{
    public class AppRole : IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
    }

}
