using System;

namespace Bank.Application.DTOs.Accounts
{
    /// <summary>Тело запроса перевода из клиента (без произвольного ToAccountId без проверки).</summary>
    public class UserBankTransferRequest
    {
        public Guid FromAccountId { get; set; }
        /// <summary>Внешний получатель: номер счёта в банке.</summary>
        public string? ToAccountNumber { get; set; }
        /// <summary>Свой счёт (личный или организации) из справочника — безопасная альтернатива вводу номера.</summary>
        public Guid? ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? RecipientInn { get; set; }
    }
}
