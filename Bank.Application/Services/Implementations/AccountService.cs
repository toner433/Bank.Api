using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bank.Domain.Interfaces;
using Bank.Domain.Models;
using Bank.Application.DTOs.Accounts;
using Bank.Application.DTOs.Operations;
using Bank.Application.Services.Interfaces;
using Bank.Application.Exceptions;

namespace Bank.Application.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDataBaseRepository _dbRepository;

        public AccountService(
            IAccountRepository accountRepository,
            IUserRepository userRepository,
            IDataBaseRepository dbRepository)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _dbRepository = dbRepository;
        }

        private async Task<AccountDto> MapToAccountDto(Account account)
        {
            var user = await _dbRepository.GetByIdAsync<User>(account.UserId);

            return new AccountDto
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                Currency = account.Currency,
                AccountType = account.AccountType,
                OpenedAt = account.OpenedAt,
                OwnerName = user.FullName 
            };
        }

        private string GenerateAccountNumber()
        {
            return "9112" + DateTime.UtcNow.Ticks.ToString().Substring(0, 10);
        }

        public async Task<AccountDto?> GetByIdAsync(Guid id)
        {
            var account = await _dbRepository.GetByIdAsync<Account>(id);
            if (account == null) return null;

            return await MapToAccountDto(account);
        }

        public async Task<List<AccountDto>> GetByUserIdAsync(Guid userId)
        {
            var accounts = await _accountRepository.GetByUserIdAsync(userId);
            var result = new List<AccountDto>();

            foreach (var account in accounts)
            {
                result.Add(await MapToAccountDto(account));
            }

            return result;
        }

        public async Task<AccountDto> CreateAccountAsync(CreateAccountRequest request)
        {
            var user = await _dbRepository.GetByIdAsync<User>(request.UserId);
            if (user == null) throw new NotFoundException("Пользователь не найден");

            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                AccountNumber = GenerateAccountNumber(),
                Balance = 0,
                Currency = request.Currency,
                AccountType = request.AccountType,
                OpenedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _dbRepository.AddAsync(account);
            return await MapToAccountDto(account);
        }

        public async Task<decimal> GetBalanceAsync(Guid accountId)
        {
            var account = await _dbRepository.GetByIdAsync<Account>(accountId);
            if (account == null) throw new NotFoundException("Счет не найден");

            return account.Balance;
        }

        public async Task<List<OperationDto>> GetAccountHistoryAsync(Guid accountId, OperationFilterDto filter)
        {
            var account = await _dbRepository.GetByIdAsync<Account>(accountId);
            if (account == null) throw new NotFoundException("Счет не найден");

            var operations = await _accountRepository.GetOperationsByAccountIdAsync(accountId);
            var result = new List<OperationDto>();

            foreach (var op in operations)
            {
                
                if (filter.FromDate.HasValue && op.CreatedAt < filter.FromDate.Value)
                    continue;

                if (filter.ToDate.HasValue && op.CreatedAt > filter.ToDate.Value)
                    continue;

                string fromNumber = "";
                if (op.FromAccountId.HasValue)
                {
                    var fromAcc = await _dbRepository.GetByIdAsync<Account>(op.FromAccountId.Value);
                    if (fromAcc != null) fromNumber = fromAcc.AccountNumber;
                }

                string toNumber = "";
                if (op.ToAccountId.HasValue)
                {
                    var toAcc = await _dbRepository.GetByIdAsync<Account>(op.ToAccountId.Value);
                    if (toAcc != null) toNumber = toAcc.AccountNumber;
                }

                result.Add(new OperationDto
                {
                    Id = op.Id,
                    Amount = op.Amount,
                    OperationType = op.OperationType.Name,
                    Description = op.Description,
                    Status = op.Status ,
                    CreatedAt = op.CreatedAt,
                    FromAccountNumber = fromNumber,
                    ToAccountNumber = toNumber
                });
            }

            result.Sort((a, b) => b.CreatedAt.CompareTo(a.CreatedAt));

            if (filter.Limit.HasValue && result.Count > filter.Limit.Value)
            {
                result = result.Take(filter.Limit.Value).ToList();
            }

            return result;
        }
    }
}