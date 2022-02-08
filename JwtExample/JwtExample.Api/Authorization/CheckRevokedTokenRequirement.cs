using JwtExample.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace JwtExample.Api.Authorization
{
    public class CheckRevokedTokenRequirement : IAuthorizationRequirement
    {
        public async Task<bool> Pass(IHttpContextAccessor contextAccessor, ITokenService tokenService)
        {
            var token = contextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization].ToString();

            var pass = !tokenService.IsTokenRevoked(token);

            return await Task.FromResult(pass);
        }
    }
}
