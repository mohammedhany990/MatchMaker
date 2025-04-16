using LinqKit;
using MatchMaker.Core.Entities;
using MatchMaker.Core.Helper;

namespace MatchMaker.Core.Specifications
{
    public class UserSpecification : BaseSpecification<AppUser>
    {
        public UserSpecification(UserParams? userParams, string currentUsername)
            : base(u => u.UserName != currentUsername)
        {

            var predicate = PredicateBuilder.New<AppUser>(Criteria!);

            if (userParams?.Gender != null)
            {
                predicate = predicate.And(u => u.Gender == userParams.Gender);
            }

            if (userParams != null)
            {
                var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
                var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));
                predicate = predicate.And(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            Criteria = predicate;

            if (userParams?.OrderBy == "created")
            {
                OrderByDescending = u => u.Created;
            }
            else
            {
                OrderByDescending = u => u.LastActive;
            }
        }
    }
}