using System;

namespace Bank.Application.DTOs.Accounts
{
  
    public class UserBankTransferRequest
    {
        public Guid FromAccountId { get; set; }
      
        public string? ToAccountNumber { get; set; }
        
        public Guid? ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? RecipientInn { get; set; }
    }
}
