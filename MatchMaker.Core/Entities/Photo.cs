using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaker.Core.Entities
{
    public class Photo : BaseEntity<int>
    {
        public string Url { get; set; }
        public string? PublicId { get; set; }
        public bool IsMain { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

    }
}
