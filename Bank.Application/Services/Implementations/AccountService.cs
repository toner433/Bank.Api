using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IOperationTypeRepository _operationTypeRepository;

        public AccountService(
            IAccountRepository accountRepository,
            IUserRepository userRepository,
            IDataBaseRepository dbRepository,
            IOperationTypeRepository operationTypeRepository)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _dbRepository = dbRepository;
            _operationTypeRepository = operationTypeRepository;
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
                OwnerName = user?.FullName ?? "Неизвестно"
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
                    OperationType = op.OperationType?.Name ?? "Неизвестно",
                    Description = op.Description ?? "",
                    Status = op.Status ?? "",
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

        public async Task<OperationDto> DepositAsync(Guid accountId, decimal amount, string description = "Пополнение счета")
        {
            if (amount <= 0)
                throw new BusinessException("Сумма должна быть больше 0");

            var account = await _dbRepository.GetByIdAsync<Account>(accountId);
            if (account == null)
                throw new NotFoundException("Счет не найден");

            var operationType = await _operationTypeRepository.GetByNameAsync("DEPOSIT");
            if (operationType == null)
                throw new BusinessException("Тип операции не найден");

            var operation = new AccountOperation
            {
                Id = Guid.NewGuid(),
                ToAccountId = account.Id,
                Amount = amount,
                OperationTypeId = operationType.Id,
                Description = description,
                Status = "Completed",
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow
            };

            account.Balance += amount;

            await _dbRepository.UpdateAsync(account);
            await _accountRepository.AddOperationAsync(operation);

            return new OperationDto
            {
                Id = operation.Id,
                Amount = operation.Amount,
                OperationType = operationType.Name,
                Description = operation.Description,
                Status = operation.Status,
                CreatedAt = operation.CreatedAt,
                ToAccountNumber = account.AccountNumber
            };
        }

        public async Task<OperationDto> WithdrawAsync(Guid accountId, decimal amount, string description = "Снятие со счета")
        {
            if (amount <= 0)
                throw new BusinessException("Сумма должна быть больше 0");

            var account = await _dbRepository.GetByIdAsync<Account>(accountId);
            if (account == null)
                throw new NotFoundException("Счет не найден");

            if (account.Balance < amount)
                throw new BusinessException("Недостаточно средств");

            var operationType = await _operationTypeRepository.GetByNameAsync("WITHDRAWAL");
            if (operationType == null)
                throw new BusinessException("Тип операции не найден");

            var operation = new AccountOperation
            {
                Id = Guid.NewGuid(),
                FromAccountId = account.Id,
                Amount = amount,
                OperationTypeId = operationType.Id,
                Description = description,
                Status = "Completed",
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow
            };

            account.Balance -= amount;

            await _dbRepository.UpdateAsync(account);
            await _accountRepository.AddOperationAsync(operation);

            return new OperationDto
            {
                Id = operation.Id,
                Amount = operation.Amount,
                OperationType = operationType.Name,
                Description = operation.Description,
                Status = operation.Status,
                CreatedAt = operation.CreatedAt,
                FromAccountNumber = account.AccountNumber
            };
        }
    }
}