using System;

namespace Bank.Application.DTOs.Admin
{
    public class AdminUserFilterRequest
    {
        public string? Status { get; set; } // "all", "blocked", "active"
        public string? Role { get; set; }   // "all", "admin", "client"
        public string? Search { get; set; } // Поиск по логину, ФИО, email
        public string? SortBy { get; set; } // "login", "fullName", "email", "createdAt"
        public string? SortOrder { get; set; } // "asc", "desc"
    }
}
