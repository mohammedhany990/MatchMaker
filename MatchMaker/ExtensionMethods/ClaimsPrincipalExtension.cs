using System.Security.Claims;

namespace MatchMaker.ExtensionMethods
{
    public static class ClaimsPrincipalExtension
    {
        public static string GetEmail(this ClaimsPrincipal user)
        {
            var email = user.FindFirstValue(ClaimTypes.Email);
            return email;
        }

        public static string GetId(this ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return id;
        }
    }
}
