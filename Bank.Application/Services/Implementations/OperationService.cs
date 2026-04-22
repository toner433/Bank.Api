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
using Bank.Application.Common;

namespace Bank.Application.Services.Implementations
{
    public class OperationService : IOperationService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IOperationTypeRepository _operationTypeRepository;
        private readonly IDataBaseRepository _dbRepository;
        private readonly IOrganizationService _organizationService;

        public OperationService(
            IAccountRepository accountRepository,
            IOperationTypeRepository operationTypeRepository,
            IDataBaseRepository dbRepository,
            IOrganizationService organizationService)
        {
            _accountRepository = accountRepository;
            _operationTypeRepository = operationTypeRepository;
            _dbRepository = dbRepository;
            _organizationService = organizationService;
        }

        private async Task<bool> UserMayAccessAccountAsync(Guid userId, Account account)
        {
            if (account.UserId.HasValue && account.UserId.Value == userId) return true;
            if (account.OrganizationId.HasValue)
                return await _organizationService.UserIsMemberAsync(account.OrganizationId.Value, userId);
            return false;
        }

        public async Task<OperationDto> TransferAsync(TransferRequest request, Guid actingUserId)
        {
            Account? toAccount = null;
            if (request.ToAccountId.HasValue)
                toAccount = await _dbRepository.GetByIdAsync<Account>(request.ToAccountId.Value);
            else if (!string.IsNullOrWhiteSpace(request.ToAccountNumber))
                toAccount = await _accountRepository.GetByAccountNumberAsync(
                    AccountNumberNormalizer.NormalizeAccountNumber(request.ToAccountNumber));

            if (toAccount == null)
                throw new NotFoundException("Счёт получателя не найден");

            var fromAccount = await _dbRepository.GetByIdAsync<Account>(request.FromAccountId);
            if (fromAccount == null) throw new NotFoundException("Счет отправителя не найден");

            if (!await UserMayAccessAccountAsync(actingUserId, fromAccount))
                throw new BusinessException("Нет прав на счёт списания");

            if (string.Equals(fromAccount.AccountType, "time_deposit", StringComparison.OrdinalIgnoreCase)
                && !request.AllowFromTimeDeposit)
                throw new BusinessException(
                    "Со счёта срочного вклада нельзя переводить напрямую. Закройте вклад на странице «Вклады» или выберите текущий счёт.");

            if (fromAccount.Id == toAccount.Id)
                throw new BusinessException("Нельзя перевести на тот же счёт");

            if (toAccount.OrganizationId.HasValue && !request.ToAccountId.HasValue)
            {
                var innFromUser = AccountNumberNormalizer.InnDigitsOnly(request.RecipientInn);
                if (string.IsNullOrEmpty(innFromUser))
                    throw new BusinessException("Для перевода на счёт юридического лица укажите ИНН получателя (проверка реквизитов).");

                var org = await _dbRepository.GetByIdAsync<Organization>(toAccount.OrganizationId.Value);
                if (org == null) throw new BusinessException("Организация получателя не найдена");

                var innOrg = AccountNumberNormalizer.InnDigitsOnly(org.Inn);
                if (!string.Equals(innFromUser, innOrg, StringComparison.Ordinal))
                    throw new BusinessException("ИНН не совпадает с владельцем счёта получателя. Проверьте номер счёта и ИНН.");
            }

            if (request.Amount <= 0)
                throw new BusinessException("Сумма перевода должна быть больше 0");

            if (fromAccount.Balance < request.Amount)
                throw new BusinessException("Недостаточно средств");

            if (fromAccount.Currency != toAccount.Currency)
                throw new BusinessException($"Валюты счетов не совпадают: {fromAccount.Currency} -> {toAccount.Currency}");

            var operationType = await _operationTypeRepository.GetByNameAsync("TRANSFER")
                ?? await _operationTypeRepository.GetByNameAsync("Transfer")
                ?? await _operationTypeRepository.GetByNameAsync("transfer");

            if (operationType == null)
            {
                operationType = new OperationType
                {
                    Id = Guid.NewGuid(),
                    Name = "TRANSFER"
                };
                await _dbRepository.AddAsync(operationType);
            }

            var operation = new AccountOperation
            {
                Id = Guid.NewGuid(),
                FromAccountId = fromAccount.Id,
                ToAccountId = toAccount.Id,
                Amount = request.Amount,
                OperationTypeId = operationType.Id,
                Description = string.IsNullOrWhiteSpace(request.Description) ? "Перевод" : request.Description.Trim(),
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
                filtered = filtered.Take(filter.Limit.Value).ToList();

            return filtered;
        }

        public async Task<List<OperationDto>> GetOrganizationOperationsAsync(Guid organizationId, Guid actingUserId, OperationFilterDto filter)
        {
            if (!await _organizationService.UserIsMemberAsync(organizationId, actingUserId))
                throw new BusinessException("Нет доступа к организации");

            var orgAccounts = await _accountRepository.GetByOrganizationIdAsync(organizationId);
            var allOperations = new List<OperationDto>();

            foreach (var account in orgAccounts)
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
                filtered = filtered.Take(filter.Limit.Value).ToList();

            return filtered;
        }
    }
}
