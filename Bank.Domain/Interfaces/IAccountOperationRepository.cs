using Bank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Interfaces
{
    public interface IAccountOperationRepository
    {
        Task<AccountOperation?> GetByIdAsync(Guid id);
        Task<List<AccountOperation>> GetByAccountIdAsync(Guid accountId);
        Task AddAsync(AccountOperation operation);
        Task UpdateAsync(AccountOperation operation);
    }
}
