using Bank.Domain.Interfaces;
using Bank.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure.Repositories
{
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        public AccountRepository(BankDbContext context) : base(context)
        {
        }

        public async Task<List<Account>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<Account?> GetByAccountNumberAsync(string accountNumber)
        {
            return await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

   

        public async Task<AccountOperation?> GetOperationByIdAsync(Guid id)
        {
            return await _context.AccountOperations
                .Include(o => o.OperationType)
                .Include(o => o.FromAccount)
                .Include(o => o.ToAccount)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<AccountOperation>> GetOperationsByAccountIdAsync(Guid accountId)
        {
            return await _context.AccountOperations
                .Include(o => o.OperationType)
                .Where(o => o.FromAccountId == accountId || o.ToAccountId == accountId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task AddOperationAsync(AccountOperation operation)
        {
            await _context.AccountOperations.AddAsync(operation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOperationAsync(AccountOperation operation)
        {
            _context.AccountOperations.Update(operation);
            await _context.SaveChangesAsync();
        }
    }
}
