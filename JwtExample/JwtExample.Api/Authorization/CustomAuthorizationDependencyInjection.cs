using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace JwtExample.Api.Authorization
{
    public static class CustomAuthorizationDependencyInjection
    {
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
        {
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.CheckRevokedToken, policy => policy.Requirements.Add(new CheckRevokedTokenRequirement()));
            });

            services.AddScoped<IAuthorizationHandler, CheckRevokedTokenHandler>();

            return services;
        }

        public static IApplicationBuilder UseCustomAuthorization(this IApplicationBuilder app) => 
            app.UseAuthentication()
                .UseAuthorization();
    }
}
