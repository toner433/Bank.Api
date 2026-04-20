using Bank.Domain.Interfaces;
using Bank.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bank.Domain.Models;

namespace Bank.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDataBaseRepository _repository;

        public AccountRepository(IDataBaseRepository repository)
        {
            _repository = repository;
        }

        private static string NormalizeAccountNumber(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            return new string(raw.Where(c => !char.IsWhiteSpace(c) && c != '\u00a0' && c != '\u2007').ToArray());
        }

        public async Task<List<Account>> GetByUserIdAsync(Guid userId)
        {
            var accounts = await _repository.GetAllAsync<Account>();
            return accounts.Where(a => a.UserId == userId).ToList();
        }

        public async Task<List<Account>> GetByOrganizationIdAsync(Guid organizationId)
        {
            var accounts = await _repository.GetAllAsync<Account>();
            return accounts.Where(a => a.OrganizationId == organizationId).ToList();
        }

        public async Task<Account?> GetByAccountNumberAsync(string accountNumber)
        {
            var want = NormalizeAccountNumber(accountNumber);
            if (string.IsNullOrEmpty(want)) return null;
            var accounts = await _repository.GetAllAsync<Account>();
            return accounts.FirstOrDefault(a => NormalizeAccountNumber(a.AccountNumber) == want);
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
