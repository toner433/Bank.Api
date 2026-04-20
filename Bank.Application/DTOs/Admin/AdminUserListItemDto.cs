using System;

namespace Bank.Application.DTOs.Admin
{
    public class AdminUserListItemDto
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsBlocked { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
