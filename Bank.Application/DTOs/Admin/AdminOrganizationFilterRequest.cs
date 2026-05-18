using System;

namespace Bank.Application.DTOs.Admin
{
    public class AdminOrganizationFilterRequest
    {
        public string? Search { get; set; }   // Поиск по названию, ИНН
        public string? SortBy { get; set; }   // "name", "inn", "createdAt"
        public string? SortOrder { get; set; } // "asc", "desc"
    }
}
