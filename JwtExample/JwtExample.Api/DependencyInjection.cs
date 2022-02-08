using JwtExample.Api.Repositories;
using JwtExample.Api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JwtExample.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProjectDependencies(this IServiceCollection services) =>
            services.AddScoped<IUserRepository, UserRepository>()
            .AddScoped<ITokenService, TokenService>();
    }
}
