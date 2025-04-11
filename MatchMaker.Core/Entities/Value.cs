using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaker.Core.Entities
{
  public  class ValueEntity : BaseEntity<int>
    {
        public string Name { get; set; }
    }
}
