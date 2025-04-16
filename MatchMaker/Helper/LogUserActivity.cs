using MatchMaker.Core.Entities;
using MatchMaker.ExtensionMethods;
using MatchMaker.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MatchMaker.Helper
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            if (context.HttpContext.User?.Identity?.IsAuthenticated != true)
            {
                return;
            }

            var id = context.HttpContext.User.GetId();
            var service = resultContext.HttpContext.RequestServices
                .GetRequiredService<IUnitOfWork>();

            var user = await service.Repository<AppUser, string>().GetAsync(id);
            if (user is null)
            {
                return;
            }
            user.LastActive = DateTime.UtcNow;
            await service.SaveAsync();
        }

    }
}
