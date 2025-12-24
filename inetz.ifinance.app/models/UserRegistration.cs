using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inetz.ifinance.app.Models
{
    public class UserRegistration
    {
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? Name { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? DeviceId { get; set; } = string.Empty;
    }

    public class VerifyBin
    {
        public string? DeviceId { get; set; } = string.Empty;
        public string? Bin { get; set; } = string.Empty;
    }
    public class VerifyBinResponse
    {
        public bool Success { get; set; }
        public bool IsLocked { get; set; }
        public int RemainingAttempts { get; set; }
    }
}
