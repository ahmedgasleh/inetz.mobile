using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inetz.ifinance.app.models
{
    public class AuthResponse
    {
        public string Token { get; set; } = "";
        public string Bin { get; set; } = "";
        public string Message { get; set; } = "";
        public bool Success => !string.IsNullOrEmpty(Token) || !string.IsNullOrEmpty(Bin);
    }
}
