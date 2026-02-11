using System;
using System.Collections.Generic;

namespace Bank.Domain.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public string AccountType { get; set; }
        public DateTime OpenedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Card> Cards { get; set; }
        public ICollection<AccountOperation> FromOperations { get; set; }
        public ICollection<AccountOperation> ToOperations { get; set; }
    }
}