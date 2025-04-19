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

        public static int GetId(this ClaimsPrincipal user)
        {
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out int userId))
                return userId;

            throw new Exception("User ID claim is missing or invalid.");
        }
        public static string GetUsername(this ClaimsPrincipal user)
        {
            var email = user.FindFirstValue(ClaimTypes.Email);
            var username = email.Split('@')[0];
            return username;
        }
    }
}
