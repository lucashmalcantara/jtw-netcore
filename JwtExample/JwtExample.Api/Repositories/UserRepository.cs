using JwtExample.Api.Models;
using System.Collections.Generic;
using System.Linq;

namespace JwtExample.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        public User Get(string username, string password)
        {
            var users = new List<User>
            {
                new User { Id = 1, Username = "lucas", Password = "lucas123", Role = "manager" },
                new User { Id = 2, Username = "alcantara", Password = "alcantara123", Role = "employee" }
            };

            return users.Where(x => x.Username.ToLower() == username.ToLower() && x.Password == password).FirstOrDefault();
        }
    }
}
