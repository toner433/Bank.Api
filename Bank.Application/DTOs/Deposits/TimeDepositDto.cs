using System;

namespace Bank.Application.DTOs.Deposits
{
    public class TimeDepositDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid DepositAccountId { get; set; }
        public string DepositAccountNumber { get; set; } = string.Empty;
        public decimal Principal { get; set; }
        public decimal AnnualRatePercent { get; set; }
        public int TermMonths { get; set; }
        public DateTime OpenedAt { get; set; }
        public DateTime MaturityDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
