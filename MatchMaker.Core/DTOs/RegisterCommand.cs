using MatchMaker.Core.Entities;
using MatchMaker.Core.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaker.Core.DTOs
{
   public class RegisterCommand
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string? Gender { get; set; }
        public string? KnownAs { get; set; }
        public string? DateOfBirth { get; set; }
    
        public string? City { get; set; }
        public string? Country { get; set; }

    }
}
