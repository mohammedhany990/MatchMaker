using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaker.Core.Helper
{
    public class PaginatedResponse<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
        public IEnumerable<T> Data { get; set; }

        public PaginatedResponse(PagedList<T> pagedList)
        {
            CurrentPage = pagedList.CurrentPage;
            PageSize = pagedList.PageSize;
            TotalCount = pagedList.TotalCount;
            TotalPages = pagedList.TotalPages;
            HasPrevious = pagedList.CurrentPage > 1;
            HasNext = pagedList.CurrentPage < pagedList.TotalPages;
            Data = pagedList;
        }
    }
}
