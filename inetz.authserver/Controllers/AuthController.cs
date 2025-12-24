using inetz.auth.dbcontext.data;
using inetz.auth.dbcontext.models;
using inetz.auth.dbcontext.services;
using inetz.authserver.helpers;
using inetz.ifinance.dbcontext.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json.Nodes;

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

        [HttpGet("exist")]
        public async Task<IActionResult> UserExist ( [FromQuery] string userId )
        {
            var user = await Task.Run(() => _db.UserProfiles.FirstOrDefault(u => u.UserId == userId));
            if (user != null)
                return Ok(new { exists = true });
            else
                return Ok(new { exists = false });
        }

        

        [HttpPost("register1")]
        public async Task<IActionResult> Register ( [FromBody] UserProfile userProfile )
        {
            if (string.IsNullOrWhiteSpace( userProfile.UserId) || string.IsNullOrWhiteSpace(userProfile.UserEmail) || string.IsNullOrWhiteSpace(userProfile.UserPassWord))
                return BadRequest("Your user ID, Email or Password cannot be null");

            if (userProfile != null)
            {
                var existRecord = await Task.Run(() => _db.UserProfiles.Any(u => u.UserId == userProfile.UserId));

                if(existRecord) return BadRequest("Your user ID (Phone Number) Already exist");


                // Hash password before saving
                userProfile.UserPassWord = PasswordHelper.HashPassword(userProfile, userProfile.UserPassWord);
                userProfile.DeviceId = Guid.NewGuid().ToString();
                userProfile.DeviceHash = PasswordHelper.HashDevice(userProfile, userProfile.DeviceId);

                _db.UserProfiles.Add(userProfile);
                _db.SaveChanges();

                // After SaveChanges, EF fills in identity columns (like Id).
                Guid newId = userProfile.Id;

                // Now you can retrieve it again if needed:
                UserProfile createdRecord = await Task.Run(() => _db.UserProfiles.FirstOrDefault(u => u.Id == newId)) ?? new UserProfile();

                //return Ok(createdRecord);

                return new JsonResult(createdRecord);

            }
            else return BadRequest();


        }

       

        [HttpPost("register2")]
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

                return new JsonResult(new JsonObject{["UserId"] = updateProfile.UserId });
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

            if (!PasswordHelper.VerifyDevice(user, user.DeviceHash, dto.DeviceId))
                return Unauthorized("Invalid device");


            //if (dto.Username != "test" || dto.Password != "pass")
            //    return Unauthorized();

            var (access, exp) = _tokens.CreateAccessToken(user.UserId, dto.DeviceId);
            var (rawRefresh, refreshEntity) = _tokens.CreateRefreshToken(user.UserId, dto.DeviceId);

            //refreshEntity.Id = new Guid();

            _db.RefreshTokens.Add(refreshEntity);
            await _db.SaveChangesAsync();

            return Ok(new AuthResponse(access, exp, rawRefresh, refreshEntity.ExpiresUtc));
        }

        [HttpPost("verifyBin")]
        public async Task<IActionResult> VerifyBin ( [FromBody] VerifyBinRequest request )
        {
            var user = await _db.UserProfiles
                .FirstOrDefaultAsync(u => u.Id == request.UserId);

            if (user == null)
                return Unauthorized();

            // 1️⃣ Expiration check
            if (user.BinExpiresAt < DateTime.UtcNow)
                return Unauthorized("BIN expired");

            // 2️⃣ Attempt limit
            if (user.BinAttempts >= 5)
                return Unauthorized("BIN locked");

            // 3️⃣ Hash user input
            string inputHash = PasswordHelper.HashBin(request.Bin);

            // 4️⃣ Constant-time comparison
            bool isValid = CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(user.BinHash),
                Convert.FromBase64String(inputHash)
            );

            if (!isValid)
            {
                user.BinAttempts++;
                await _db.SaveChangesAsync();
                return Unauthorized("Invalid BIN");
            }

            // 5️⃣ Success → mark device trusted
            user.BinHash = null;
            user.BinExpiresAt = DateTime.Now;
            user.BinAttempts = 0;
            user.DeviceVerified = true;

            await _db.SaveChangesAsync();

            return Ok();
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
