using System;

namespace Bank.Domain.Models
{
    /// <summary>Срочный вклад: отдельный счёт и срок с фиксированной ставкой.</summary>
    public class TimeDeposit
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        public Guid? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        public Guid DepositAccountId { get; set; }
        public Account DepositAccount { get; set; } = null!;
        public decimal Principal { get; set; }
        public decimal AnnualRatePercent { get; set; }
        public int TermMonths { get; set; }
        public DateTime OpenedAt { get; set; }
        public DateTime MaturityDate { get; set; }
        /// <summary>Active, Closed</summary>
        public string Status { get; set; } = "Active";
    }
}
