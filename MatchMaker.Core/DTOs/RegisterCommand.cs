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
    }
}
