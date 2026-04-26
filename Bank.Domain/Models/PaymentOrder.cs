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
        public string? DocumentNumber { get; set; }
        public DateTime? DocumentDate { get; set; }
        public decimal Amount { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string? RecipientInn { get; set; }
        public string? RecipientKpp { get; set; }
        public string? RecipientAccountNumber { get; set; }
        public string? RecipientBankName { get; set; }
        public string? RecipientBankBik { get; set; }
        public int? PaymentPriority { get; set; }
        public string? PaymentType { get; set; }
        public string? VatType { get; set; }
        public decimal? VatAmount { get; set; }
        public string Purpose { get; set; } = string.Empty;
        /// <summary>Draft, Signed, Executed, Cancelled</summary>
        public string Status { get; set; } = "Draft";
        public string? SignatureValue { get; set; }
        public string? SignerCertificateThumbprint { get; set; }
        public DateTime? SignedAt { get; set; }
        public Guid CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExecutedAt { get; set; }
    }
}
