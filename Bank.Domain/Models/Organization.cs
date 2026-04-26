using System;
using System.Collections.Generic;

namespace Bank.Domain.Models
{
    public class Organization
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Inn { get; set; } = string.Empty;
        public string? Kpp { get; set; }
        public string LegalAddress { get; set; } = string.Empty;
        public string? PublicKeyPem { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<OrganizationMember> Members { get; set; } = new List<OrganizationMember>();
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<PaymentOrder> PaymentOrders { get; set; } = new List<PaymentOrder>();
        public ICollection<TimeDeposit> TimeDeposits { get; set; } = new List<TimeDeposit>();
    }
}
