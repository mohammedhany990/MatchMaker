using System.Linq.Expressions;

namespace MatchMaker.Core.Specifications
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; set; }
        List<Expression<Func<T, object>>> Includes { get; set; }

        Expression<Func<T, object>> OrderByAscending { get; set; }
        Expression<Func<T, object>> OrderByDescending { get; set; }
    }
}
