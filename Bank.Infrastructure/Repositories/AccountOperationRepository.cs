using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.Domain.Models;
using Bank.Domain.Interfaces;
using Bank.Infrastructure.Context;

namespace Bank.Infrastructure.Repositories
{
    public class AccountOperationRepository : IAccountOperationRepository
    {
        private readonly BankDbContext _context;

        public AccountOperationRepository(BankDbContext context)
        {
            _context = context;
        }

        public async Task<AccountOperation?> GetByIdAsync(Guid id)
        {
            return await _context.AccountOperations
                .Include(o => o.OperationType)
                .Include(o => o.FromAccount)
                .Include(o => o.ToAccount)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<AccountOperation>> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.AccountOperations
                .Include(o => o.OperationType)
                .Where(o => o.FromAccountId == accountId || o.ToAccountId == accountId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(AccountOperation operation)
        {
            await _context.AccountOperations.AddAsync(operation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AccountOperation operation)
        {
            _context.AccountOperations.Update(operation);
            await _context.SaveChangesAsync();
        }
    }
}