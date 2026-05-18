using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.DTOs.Accounts
{
    public class CreateAccountRequest
    {
        
        public Guid? OrganizationId { get; set; }
        public string Currency { get; set; } = "BYN";
        public string AccountType { get; set; } = "current";
    }
}
