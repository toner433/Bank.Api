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
    public class OperationService : IOperationService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOperationTypeRepository _operationTypeRepository;
        private readonly IDataBaseRepository _dbRepository;

        public OperationService(
            IAccountRepository accountRepository,
            IOperationTypeRepository operationTypeRepository,
            IDataBaseRepository dbRepository)
        {
            _accountRepository = accountRepository;
            _operationTypeRepository = operationTypeRepository;
            _dbRepository = dbRepository;
        }

        public async Task<OperationDto> TransferAsync(TransferRequest request)
        {
            var fromAccount = await _dbRepository.GetByIdAsync<Account>(request.FromAccountId);
            var toAccount = await _dbRepository.GetByIdAsync<Account>(request.ToAccountId);

            if (fromAccount == null) throw new NotFoundException("Счет отправителя не найден");
            if (toAccount == null) throw new NotFoundException("Счет получателя не найден");
            if (fromAccount.Balance < request.Amount) throw new BusinessException("Недостаточно средств");
            if (fromAccount.Currency != toAccount.Currency) throw new BusinessException("Валюты счетов не совпадают");

            var operationType = await _operationTypeRepository.GetByNameAsync("TRANSFER");
            if (operationType == null) throw new BusinessException("Тип операции не найден");

            var operation = new AccountOperation
            {
                Id = Guid.NewGuid(),
                FromAccountId = fromAccount.Id,
                ToAccountId = toAccount.Id,
                Amount = request.Amount,
                OperationTypeId = operationType.Id,
                Description = request.Description,
                Status = "Completed",
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow
            };

            fromAccount.Balance -= request.Amount;
            toAccount.Balance += request.Amount;

            await _dbRepository.UpdateAsync(fromAccount);
            await _dbRepository.UpdateAsync(toAccount);
            await _accountRepository.AddOperationAsync(operation);

            return new OperationDto
            {
                Id = operation.Id,
                Amount = operation.Amount,
                OperationType = operationType.Name,
                Description = operation.Description,
                Status = operation.Status,
                CreatedAt = operation.CreatedAt,
                FromAccountNumber = fromAccount.AccountNumber,
                ToAccountNumber = toAccount.AccountNumber
            };
        }

        public async Task<OperationDto?> GetOperationByIdAsync(Guid id)
        {
            var operation = await _accountRepository.GetOperationByIdAsync(id);
            if (operation == null) return null;

            var fromAccount = operation.FromAccountId.HasValue
                ? await _dbRepository.GetByIdAsync<Account>(operation.FromAccountId.Value)
                : null;

            var toAccount = operation.ToAccountId.HasValue
                ? await _dbRepository.GetByIdAsync<Account>(operation.ToAccountId.Value)
                : null;

            return new OperationDto
            {
                Id = operation.Id,
                Amount = operation.Amount,
                OperationType = operation.OperationType?.Name ?? "Неизвестно",
                Description = operation.Description ?? "",
                Status = operation.Status ?? "",
                CreatedAt = operation.CreatedAt,
                FromAccountNumber = fromAccount?.AccountNumber ?? "",
                ToAccountNumber = toAccount?.AccountNumber ?? ""
            };
        }

        public async Task<List<OperationDto>> GetUserOperationsAsync(Guid userId, OperationFilterDto filter)
        {
            var accounts = await _accountRepository.GetByUserIdAsync(userId);
            var allOperations = new List<OperationDto>();

            foreach (var account in accounts)
            {
                var operations = await _accountRepository.GetOperationsByAccountIdAsync(account.Id);
                foreach (var op in operations)
                {
                    var dto = await GetOperationByIdAsync(op.Id);
                    if (dto != null)
                        allOperations.Add(dto);
                }
            }

            var filtered = allOperations
                .Where(x => !filter.FromDate.HasValue || x.CreatedAt >= filter.FromDate.Value)
                .Where(x => !filter.ToDate.HasValue || x.CreatedAt <= filter.ToDate.Value)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            if (filter.Limit.HasValue && filtered.Count > filter.Limit.Value)
            {
                filtered = filtered.Take(filter.Limit.Value).ToList();
            }

            return filtered;
        }

    }
}