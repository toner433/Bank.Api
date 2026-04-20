using System;

namespace Bank.Application.DTOs.PaymentOrders
{
    public class CreatePaymentOrderRequest
    {
        public Guid OrganizationId { get; set; }
        public Guid FromAccountId { get; set; }
        public decimal Amount { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string? RecipientInn { get; set; }
        public string? RecipientAccountNumber { get; set; }
        public string Purpose { get; set; } = string.Empty;
    }
}
