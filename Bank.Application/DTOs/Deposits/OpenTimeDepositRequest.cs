using System;

namespace Bank.Application.DTOs.Deposits
{
    public class OpenTimeDepositRequest
    {
        /// <summary>Либо физлицо (счёт пользователя), либо корпоративный счёт — укажите organizationId.</summary>
        public Guid? OrganizationId { get; set; }
        public Guid FromAccountId { get; set; }
        public decimal Amount { get; set; }
        /// <summary>Срок в месяцах: 3, 6 или 12.</summary>
        public int TermMonths { get; set; }
    }
}
