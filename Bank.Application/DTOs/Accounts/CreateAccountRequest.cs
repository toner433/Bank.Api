using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.DTOs.Accounts
{
    public class CreateAccountRequest
    {
        /// <summary>Если задано — открывается корпоративный счёт (только директор организации).</summary>
        public Guid? OrganizationId { get; set; }
        public string Currency { get; set; } = "BYN";
        public string AccountType { get; set; } = "current";
    }
}
