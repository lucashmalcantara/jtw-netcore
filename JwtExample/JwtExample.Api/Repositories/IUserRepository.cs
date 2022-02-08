using JwtExample.Api.Models;

namespace JwtExample.Api.Repositories
{
    public interface IUserRepository
    {
        User Get(string username, string password);
    }
}
