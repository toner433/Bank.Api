using System;
using System.Collections.Generic;

namespace Bank.Domain.Models
{
    public class OperationType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<AccountOperation> AccountOperations { get; set; }
    }
}