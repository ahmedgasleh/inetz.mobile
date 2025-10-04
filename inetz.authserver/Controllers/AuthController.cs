using inetz.auth.dbcontext.models;
using inetz.auth.dbcontext.services;
using inetz.authserver.helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using inetz.auth.dbcontext.data;
using Microsoft.EntityFrameworkCore;

namespace inetz.authserver.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly ITokenService _tokens;
        private readonly AuthDbContext _db;

        public AuthController ( ITokenService tokens, AuthDbContext db )
        {
            _tokens = tokens;
            _db = db;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register ( [FromBody] UserProfile userProfile )
        {
            if (userProfile != null)
            {
                // Hash password before saving
                userProfile.UserPassWord = PasswordHelper.HashPassword(userProfile, userProfile.UserPassWord);

                _db.UserProfiles.Add(userProfile);
                _db.SaveChanges();

                // After SaveChanges, EF fills in identity columns (like Id).
                Guid newId = userProfile.Id;

                // Now you can retrieve it again if needed:
                var createdRecord = await Task.Run(() => _db.UserProfiles.FirstOrDefault(u => u.Id == newId));

                return Ok(createdRecord);

            }
            else return BadRequest();


        }

        [HttpPost("updateProfile")]
        public async Task<IActionResult> Register ( [FromBody] UpdateProfile updateProfile )
        {
            if (updateProfile != null)
            {
                var createdRecord = await Task.Run(() => _db.UserProfiles.FirstOrDefault(u => u.UserId == updateProfile.UserId));
                if (createdRecord != null)
                {
                    createdRecord.FirstName = updateProfile.FirstName;
                    createdRecord.LastName = updateProfile.LastName;
                    createdRecord.Address = updateProfile.Address;
                    _db.SaveChanges();
                }

                return Ok();
            }
            else return BadRequest();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login ( [FromBody] LoginDto dto )
        {


            var user = _db.UserProfiles.FirstOrDefault(u => u.UserId == dto.UserId);
            if (user == null)
                return Unauthorized("User not found");

            if (!PasswordHelper.VerifyPassword(user, user.UserPassWord, dto.Password))
                return Unauthorized("Invalid password");


            //if (dto.Username != "test" || dto.Password != "pass")
            //    return Unauthorized();

            var (access, exp) = _tokens.CreateAccessToken(user.UserId, dto.DeviceId);
            var (rawRefresh, refreshEntity) = _tokens.CreateRefreshToken(user.UserId, dto.DeviceId);

            //refreshEntity.Id = new Guid();

            _db.RefreshTokens.Add(refreshEntity);
            await _db.SaveChangesAsync();

            return Ok(new AuthResponse(access, exp, rawRefresh, refreshEntity.ExpiresUtc));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh ( [FromBody] RefreshDto dto )
        {
            var hash = _tokens.Hash(dto.RefreshToken);
            var token = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash);

            bool IsActive = token.RevokedUtc == null && DateTime.UtcNow < token.ExpiresUtc;

            if (token is null || !IsActive || token.DeviceId != dto.DeviceId)
                return Unauthorized();

            token.RevokedUtc = DateTime.UtcNow;

            var (access, exp) = _tokens.CreateAccessToken(token.UserId, token.DeviceId);
            var (rawNew, newEntity) = _tokens.CreateRefreshToken(token.UserId, token.DeviceId);
            token.ReplacedByTokenHash = newEntity.TokenHash;

            _db.RefreshTokens.Add(newEntity);
            await _db.SaveChangesAsync();

            return Ok(new AuthResponse(access, exp, rawNew, newEntity.ExpiresUtc));
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me ()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";
            return Ok(new { userId, device = User.FindFirstValue("device_id") });
        }
    }

    public record LoginDto ( string UserId, string Password, string DeviceId );
    public record RefreshDto ( string RefreshToken, string DeviceId );
    public record AuthResponse ( string AccessToken, DateTime AccessTokenExpiresUtc, string RefreshToken, DateTime RefreshTokenExpiresUtc );

}
