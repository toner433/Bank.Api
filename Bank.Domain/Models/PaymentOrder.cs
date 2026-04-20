using System;

namespace Bank.Domain.Models
{
    public class PaymentOrder
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;
        public Guid FromAccountId { get; set; }
        public Account FromAccount { get; set; } = null!;
        public decimal Amount { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string? RecipientInn { get; set; }
        public string? RecipientAccountNumber { get; set; }
        public string Purpose { get; set; } = string.Empty;
        /// <summary>Draft, Executed, Cancelled</summary>
        public string Status { get; set; } = "Draft";
        public Guid CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExecutedAt { get; set; }
    }
}
