using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inetz.ifinance.app.Models
{
    public class LoginRequest
    {
        public string? UserId { get; set; }
        public string? Password { get; set; }
        public string? DeviceId { get; set; }
    }

    public class VerifyBinRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string Bin { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public DateTime AccessTokenExpiresUtc { get; set; }

        public DateTime RefreshTokenExpiresUtc { get; set; }
    }

    public class BinResponse
    {        
        public string Bin { get; set; } = string.Empty;
    }

    public class CreateBinRequest
    {
        public string UserId { get; set; } = "";
        public string DeviceId { get; set; } = "";
    }
}
