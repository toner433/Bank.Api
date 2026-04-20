using System;

namespace Bank.Application.DTOs.Organizations
{
    public class OrganizationMemberDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserLogin { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
    }
}
