using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaker.Core.Helper
{
    public class MessageParams : PaginationParams
    {
        public string? Username { get; set; }
        public string Container { get; set; } = "Unread";
    }
}
