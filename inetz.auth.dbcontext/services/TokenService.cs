using inetz.auth.dbcontext.models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace inetz.auth.dbcontext.services
{
    public interface ITokenService
    {
        (string access, DateTime exp) CreateAccessToken ( string userId, string deviceId );
        (string raw, RefreshToken entity) CreateRefreshToken ( string userId, string deviceId );
        string Hash ( string input );
    }

    public sealed class TokenService : ITokenService
    {
        private readonly IConfiguration _cfg;
        public TokenService ( IConfiguration cfg ) => _cfg = cfg;

        public (string access, DateTime exp) CreateAccessToken ( string userId, string deviceId )
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg ["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var exp = DateTime.UtcNow.AddMinutes(int.Parse(_cfg ["Jwt:AccessTokenMinutes"]!));

            var claims = new []
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim("device_id", deviceId)
        };

            var jwt = new JwtSecurityToken(
                issuer: _cfg ["Jwt:Issuer"],
                audience: _cfg ["Jwt:Audience"],
                claims: claims,
                expires: exp,
                signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(jwt), exp);
        }

        public (string raw, RefreshToken entity) CreateRefreshToken ( string userId, string deviceId )
        {
            var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var entity = new RefreshToken
            {
                UserId = userId,
                DeviceId = deviceId,
                TokenHash = Hash(raw),
                ExpiresUtc = DateTime.UtcNow.AddDays(int.Parse(_cfg ["Jwt:RefreshTokenDays"]!))
            };
            return (raw, entity);
        }

        public string Hash ( string input )
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }
    }

}
