using JwtExample.Api.Authorization;
using JwtExample.Api.Models;
using JwtExample.Api.Repositories;
using JwtExample.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace JwtExample.Api.Controllers
{
    [ApiController]
    [Route("v1")]
    public class HomeController : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        public IActionResult Authenticate([FromServices] IUserRepository userRepository,
            [FromServices] ITokenService tokenService,
            [FromBody] User model)
        {
            var user = userRepository.Get(model.Username, model.Password);

            if (user == null)
            {
                var authenticationErrorResult = new
                {
                    Message = "Username or password is invalid"
                };

                return Unauthorized(authenticationErrorResult);
            }

            var token = tokenService.GenerateToken(user);

            user.Password = "";

            var authenticationSuccessResult = new
            {
                User = user,
                Token = token
            };

            return Ok(authenticationSuccessResult);
        }

        [HttpPost]
        [Route("logout")]
        [Authorize]
        public IActionResult Logout([FromServices] ITokenService tokenService)
        {
            var token = Request.Headers[HeaderNames.Authorization].ToString();

            tokenService.RevokeToken(token);

            return NoContent();
        }

        [HttpGet]
        [Route("anonymous")]
        [AllowAnonymous]
        public string Anonymous() => "Anonymous";

        [HttpGet]
        [Route("authenticated")]
        [Authorize(Policy = PolicyNames.CheckRevokedToken)]
        public string Authenticated() => $"Authenticated - {User.Identity.Name}";

        [HttpGet]
        [Route("employee")]
        [Authorize(Policy = PolicyNames.CheckRevokedToken, Roles = "employee,manager")]
        public string Employee() => "Employee";

        [HttpGet]
        [Route("manager")]
        [Authorize(Policy = PolicyNames.CheckRevokedToken, Roles = "manager")]
        public string Manager() => "Manager";
    }
}
