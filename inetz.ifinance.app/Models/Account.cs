using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inetz.ifinance.app.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string BalanceType { get; set; } = string.Empty;
        public string BalanceCurrency { get; set; } = string.Empty;
        public int LogoId { get; set; }
    }
}
