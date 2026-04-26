namespace Bank.Application.DTOs.PaymentOrders
{
    public class SignPaymentOrderRequest
    {
        public bool DeviceDetected { get; set; }
        public string SignatureValue { get; set; } = string.Empty;
        public string? CertificateThumbprint { get; set; }
    }
}

