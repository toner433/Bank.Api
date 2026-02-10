using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.Domain.Models;

namespace Bank.Domain.Models
{
    public class AccountOperation 
    {
        public Guid id { get; set; }

        public Account FromAccount { get; set; }
        public Guid FromAccoiuntId  { get; set; }
        public Account ToAccount { get; set; }
        public Guid ToAccountId { get; set; }
        public string amount { get; set; }
        public OperationType OperationType { get; set; }
        public Guid OperationTypeId { get; set; }
        public string description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CompletedAt { get; set; }

    }
}
