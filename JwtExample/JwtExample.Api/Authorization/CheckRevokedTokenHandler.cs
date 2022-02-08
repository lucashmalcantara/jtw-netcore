using JwtExample.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace JwtExample.Api.Authorization
{

    public class CheckRevokedTokenHandler : AuthorizationHandler<CheckRevokedTokenRequirement>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ITokenService _tokenService;

        public CheckRevokedTokenHandler(IHttpContextAccessor contextAccessor, ITokenService tokenService)
        {
            _contextAccessor = contextAccessor;
            _tokenService = tokenService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CheckRevokedTokenRequirement requirement)
        {
            if (await requirement.Pass(_contextAccessor, _tokenService))
                context.Succeed(requirement);
        }
    }
}
