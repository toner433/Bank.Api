namespace Bank.Application.DTOs.Admin
{
    public class UpdateAdminAccountRequest
    {
        public string? AdminComment { get; set; }
        public decimal? Balance { get; set; }
        public string? Currency { get; set; }
        public string? AccountType { get; set; }
        public bool? IsBlocked { get; set; }
    }
}

