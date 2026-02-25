using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.DTOs.Accounts
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public DateTime OpenedAt { get; set; }
        public string OwnerName { get; set; } = string.Empty;
    }
}
