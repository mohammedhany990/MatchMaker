using FluentValidation;
using MatchMaker.Core.Helper;
using System.Text.Json;

namespace MatchMaker.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = 500;


                List<string> errors = new();

                if (ex is ValidationException validationEx)
                {
                    errors = validationEx.Errors.Select(e => e.ErrorMessage).ToList();
                }
                else
                {
                    errors.Add(ex.Message);
                }

                var response = new BaseResponse<object>(
                    statusCode: 500,
                    success: false,
                    message: "Validation failed.",
                    data: null
                )
                {
                    Errors = errors
                };

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(response, options);
                await httpContext.Response.WriteAsync(json);
            }
        }
    }
}
