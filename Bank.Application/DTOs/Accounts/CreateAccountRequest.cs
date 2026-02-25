using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.DTOs.Accounts
{
    public class CreateAccountRequest
    {
        public Guid UserId { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
    }
}
