using System;

namespace Bank.Application.DTOs.Admin
{
    public class AdminAccountFilterRequest
    {
        public string? Currency { get; set; }    // "all", "BYN", "USD", "EUR", "RUB"
        public string? AccountType { get; set; } // "all", "Debit", "corporate_current", "time_deposit"
        public string? Status { get; set; }      // "all", "blocked", "active"
        public string? Search { get; set; }      // Поиск по номеру счета, владельцу
        public string? SortBy { get; set; }      // "accountNumber", "balance", "createdAt", "owner"
        public string? SortOrder { get; set; }   // "asc", "desc"
    }
}
