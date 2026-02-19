using Bank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Interfaces
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<List<Account>> GetByUserIdAsync(Guid userId);
        Task<Account?> GetByAccountNumberAsync(string accountNumber);
        Task<AccountOperation?> GetOperationByIdAsync(Guid id);
        Task<List<AccountOperation>> GetOperationsByAccountIdAsync(Guid accountId);
        Task AddOperationAsync(AccountOperation operation);
        Task UpdateOperationAsync(AccountOperation operation);
    }
}
