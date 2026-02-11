using System;

namespace Bank.Domain.Models
{
    public class AccountOperation
    {
        public Guid Id { get; set; }
        public Guid? FromAccountId { get; set; }
        public Account FromAccount { get; set; }
        public Guid? ToAccountId { get; set; }
        public Account ToAccount { get; set; }
        public decimal Amount { get; set; }
        public Guid OperationTypeId { get; set; }
        public OperationType OperationType { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}