using Bank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(Guid id);
        Task<List<Account>> GetByUserIdAsync(Guid userId);
        Task<Account?> GetByAccountNumberAsync(string accountNumber);
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteByIdAsync(Guid id);
    }
}
