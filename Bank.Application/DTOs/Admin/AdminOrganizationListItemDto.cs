using System;

namespace Bank.Application.DTOs.Admin
{
    public class AdminOrganizationListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Inn { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
