using Newtonsoft.Json;

namespace MatchMaker.Core.DTOs
{
    public class UserResponse
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string KnownAs { get; set; }
        public string PictureUrl { get; set; }

        public DateTime? ExpiresOn { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiration { get; set; }
    }
}
