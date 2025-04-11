using MatchMaker.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaker.Core.DTOs
{
   public class PhotoDto
    {
        public string Id { get; set; }
        public string? Url { get; set; }
        public bool IsMain { get; set; }
    }
}
