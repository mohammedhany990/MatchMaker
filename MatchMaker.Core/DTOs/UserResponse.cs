using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaker.Core.DTOs
{
    public class UserResponse
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string KnownAs { get; set; }
        public string PictureUrl { get; set; }

    }
}
