using System;
using System.Collections.Generic;

namespace Bank.Domain.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string PassportNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Account> Accounts { get; set; }
        public ICollection<Session> Sessions { get; set; }
        public ICollection<VerificationCode> VerificationCodes { get; set; }
        public ICollection<Card> Cards { get; set; }
    }
}