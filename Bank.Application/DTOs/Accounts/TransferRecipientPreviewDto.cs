namespace Bank.Application.DTOs.Accounts
{
    public class TransferRecipientPreviewDto
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public string RecipientKind { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Inn { get; set; }
    }
}
