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
    public class AccountRepository : IAccountRepository
    {
        private readonly IDataBaseRepository _repository;

        public AccountRepository(IDataBaseRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Account>> GetByUserIdAsync(Guid userId)
        {
            var accounts = await _repository.GetAllAsync<Account>();
            return accounts.Where(a => a.UserId == userId).ToList();
        }

        public async Task<Account?> GetByAccountNumberAsync(string accountNumber)
        {
            var accounts = await _repository.GetAllAsync<Account>();
            return accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        }

        public async Task<AccountOperation?> GetOperationByIdAsync(Guid id)
        {
            var operations = await _repository.GetAllAsync<AccountOperation>();
            return operations.FirstOrDefault(o => o.Id == id);
        }

        public async Task<List<AccountOperation>> GetOperationsByAccountIdAsync(Guid accountId)
        {
            var operations = await _repository.GetAllAsync<AccountOperation>();
            return operations.Where(o => o.FromAccountId == accountId || o.ToAccountId == accountId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
        }

        public async Task AddOperationAsync(AccountOperation operation)
        {
            await _repository.AddAsync(operation);
        }
    }
}
