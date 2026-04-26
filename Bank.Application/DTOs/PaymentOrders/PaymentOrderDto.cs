using System;

namespace Bank.Application.DTOs.PaymentOrders
{
    public class PaymentOrderDto
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid FromAccountId { get; set; }
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
        public string Status { get; set; } = string.Empty;
        public string? SignerCertificateThumbprint { get; set; }
        public DateTime? SignedAt { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExecutedAt { get; set; }
    }
}
