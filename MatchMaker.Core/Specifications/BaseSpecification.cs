using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaker.Core.Specifications
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderByAscending { get; set; }
        public Expression<Func<T, object>> OrderByDescending { get; set; }
        public BaseSpecification() 
        {
            
        }
        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        //public void ApplyOrderByAsc(Expression<Func<T, object>> orderExpression)
        //{
        //    OrderByAscending = orderExpression;
        //}
        //public void ApplyOrderByDesc(Expression<Func<T, object>> orderExpression)
        //{
        //    OrderByDescending = orderExpression;
        //}
    }
}
