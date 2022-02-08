using JwtExample.Api.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JwtExample.Api.Services
{
    public class TokenService : ITokenService
    {
        // ATTENTION: Please consider replacing with a distributed cache (eg Redis).
        private readonly IMemoryCache _memoryCache;

        public TokenService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public void RevokeToken(string token)
        {
            var rawToken = token.Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.ReadToken(rawToken) as JwtSecurityToken;
            var tokenExpiryDate = securityToken.ValidTo;

            // If there is no valid `exp` claim then `ValidTo` returns DateTime.MinValue
            if (tokenExpiryDate == DateTime.MinValue) throw new Exception("Could not get exp claim from token");

            // If the token is in the past then you can't use it
            if (tokenExpiryDate < DateTime.UtcNow) throw new Exception($"Token expired on: {tokenExpiryDate}");

            // Token is valid
            var isRevoked = true;
            var tokenHash = CreateSha512Hash(token);

            _memoryCache.Set(tokenHash, isRevoked, tokenExpiryDate);
        }

        public bool IsTokenRevoked(string token)
        {
            var tokenHash = CreateSha512Hash(token);
            var valueExists = _memoryCache.TryGetValue(tokenHash, out bool isRevoked);

            if (!valueExists)
                return false;

            return isRevoked;
        }

        private string CreateSha512Hash(string login)
        {
            var sha512Hash = SHA512.Create();

            var hashInByteArrayFormat = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(login));

            sha512Hash.Dispose();

            var stringBuilder = new StringBuilder();

            for (var i = 0; i < hashInByteArrayFormat.Length; i++)
            {
                stringBuilder.Append(hashInByteArrayFormat[i].ToString("x2"));
            }

            var hashInStringFormat = stringBuilder.ToString();

            return hashInStringFormat;
        }
    }
}
