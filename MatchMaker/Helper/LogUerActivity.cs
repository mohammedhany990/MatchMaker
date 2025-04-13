using MatchMaker.ExtensionMethods;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MatchMaker.Helper
{
    public class LogUerActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            if (context.HttpContext.User.Identity.IsAuthenticated != true)
            {
                return;
            }
            var userId = context.HttpContext.User.FindFirst("id")?.Value;
            var username = context.HttpContext.User.GetEmail().Split("@")[0];
            //var repo = resultContext.HttpContext.RequestServices
            //    .GetRequiredService<>
        }
    }
}
