using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaker.Core.Helper
{
    public class UserParams
    {
        private const int MaxPageSize = 50;
        private const int DefaultPageSize = 10;
        private const int MinPageNumber = 1;

        private int _pageNumber = MinPageNumber;
        private int _pageSize = DefaultPageSize;

        public string? Gender{ get; set; }
        public string? CurrentUsername{ get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 100;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = (value < MinPageNumber) ? MinPageNumber : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize :
                (value < 1) ? DefaultPageSize : value;
        }

        public string OrderBy { get; set; } = "lastActive";
    }
}
