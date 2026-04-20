using System;

namespace Bank.Application.DTOs.Deposits
{
    public class CloseTimeDepositRequest
    {
        public Guid TimeDepositId { get; set; }
        public Guid TargetAccountId { get; set; }
    }
}
