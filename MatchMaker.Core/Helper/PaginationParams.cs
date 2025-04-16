namespace MatchMaker.Core.Helper
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        private const int DefaultPageSize = 10;
        private const int MinPageNumber = 1;

        private int _pageNumber = MinPageNumber;
        private int _pageSize = DefaultPageSize;

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
    }
}
