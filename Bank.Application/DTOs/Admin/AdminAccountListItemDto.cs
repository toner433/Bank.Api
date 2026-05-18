using System;

namespace Bank.Application.DTOs.Admin
{
    public class AdminAccountListItemDto
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public bool IsBlocked { get; set; }
        public string? AdminComment { get; set; }
        public string Owner { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

