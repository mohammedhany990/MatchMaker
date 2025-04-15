using System.Reflection;
using FluentValidation;
using MatchMaker.Core.Entities;
using MatchMaker.Errors;
using MatchMaker.Helper;
using MatchMaker.Infrastructure.Identity;
using MatchMaker.Infrastructure.Implementations;
using MatchMaker.Infrastructure.Interfaces;
using MatchMaker.Service;
using MatchMaker.Service.Abstracts;
using MatchMaker.Service.Implementations;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MatchMaker.ExtensionMethods
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped(typeof(IAuthService), typeof(AuthService));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IPhotoService), typeof(PhotoService));
            services.AddScoped(typeof(IUserService), typeof(UserService));
            services.AddScoped(typeof(LogUserActivity));

            services.AddAutoMapper(typeof(MappingProfile));



            // Configure MediatR
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;

                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(e => e.Value.Errors)
                        .Select(m => m.ErrorMessage)
                        .ToList();

                    return new BadRequestObjectResult(new ValidationErrorResponse(errors));
                };
            });

            return services;
        }
    }
}
