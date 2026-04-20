using System;

namespace Bank.Domain.Models
{
    public class OrganizationMember
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        /// <summary>Director или Accountant</summary>
        public string Role { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
    }
}
