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
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IDataBaseRepository _dbRepository;
        private readonly IOperationTypeRepository _operationTypeRepository;
        private readonly IOrganizationService _organizationService;

        public AccountService(
            IAccountRepository accountRepository,
            IDataBaseRepository dbRepository,
            IOperationTypeRepository operationTypeRepository,
            IOrganizationService organizationService)
        {
            _accountRepository = accountRepository;
            _dbRepository = dbRepository;
            _operationTypeRepository = operationTypeRepository;
            _organizationService = organizationService;
        }

        private async Task<bool> UserMayAccessAccountAsync(Guid userId, Account account)
        {
            if (account.UserId.HasValue && account.UserId.Value == userId) return true;
            if (account.OrganizationId.HasValue)
                return await _organizationService.UserIsMemberAsync(account.OrganizationId.Value, userId);
            return false;
        }

        private async Task EnsureAccessAsync(Guid userId, Account? account)
        {
            if (account == null) throw new NotFoundException("Счет не найден");
            if (!await UserMayAccessAccountAsync(userId, account))
                throw new BusinessException("Нет доступа к счёту");
        }

        private async Task<AccountDto> MapToAccountDto(Account account)
        {
            string ownerName = "Неизвестно";
            Guid? orgId = null;
            string? orgName = null;

            if (account.UserId.HasValue)
            {
                var user = await _dbRepository.GetByIdAsync<User>(account.UserId.Value);
                if (user != null) ownerName = user.FullName;
            }

            if (account.OrganizationId.HasValue)
            {
                var org = await _dbRepository.GetByIdAsync<Organization>(account.OrganizationId.Value);
                if (org != null)
                {
                    ownerName = org.Name;
                    orgId = org.Id;
                    orgName = org.Name;
                }
            }

            return new AccountDto
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                Currency = account.Currency,
                AccountType = account.AccountType,
                OpenedAt = account.OpenedAt,
                OwnerName = ownerName,
                OrganizationId = orgId,
                OrganizationName = orgName
            };
        }

        private string GenerateAccountNumber()
        {
            return "9112" + DateTime.UtcNow.Ticks.ToString().Substring(0, 10);
        }

        public async Task<AccountDto?> GetByIdAsync(Guid id, Guid actingUserId)
        {
            var account = await _dbRepository.GetByIdAsync<Account>(id);
            if (account == null) return null;
            await EnsureAccessAsync(actingUserId, account);
            return await MapToAccountDto(account);
        }

        public async Task<List<AccountDto>> GetByUserIdAsync(Guid userId, Guid actingUserId)
        {
            if (userId != actingUserId)
                throw new BusinessException("Нельзя запрашивать чужие счета");
            return await GetAccessibleAccountsAsync(actingUserId);
        }

        public async Task<List<AccountDto>> GetAccessibleAccountsAsync(Guid actingUserId)
        {
            var result = new List<AccountDto>();

            foreach (var a in await _accountRepository.GetByUserIdAsync(actingUserId))
                result.Add(await MapToAccountDto(a));

            var members = await _dbRepository.GetAllAsync<OrganizationMember>();
            var orgIds = members.Where(m => m.UserId == actingUserId).Select(m => m.OrganizationId).Distinct();
            foreach (var orgId in orgIds)
            {
                foreach (var a in await _accountRepository.GetByOrganizationIdAsync(orgId))
                    result.Add(await MapToAccountDto(a));
            }

            return result.OrderBy(x => x.AccountNumber).ToList();
        }

        public async Task<AccountDto> CreateAccountAsync(CreateAccountRequest request, Guid actingUserId)
        {
            if (request.OrganizationId.HasValue)
            {
                if (!await _organizationService.UserIsDirectorAsync(request.OrganizationId.Value, actingUserId))
                    throw new BusinessException("Только директор может открывать корпоративные счета");

                var account = new Account
                {
                    Id = Guid.NewGuid(),
                    UserId = null,
                    OrganizationId = request.OrganizationId,
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

            var personal = new Account
            {
                Id = Guid.NewGuid(),
                UserId = actingUserId,
                OrganizationId = null,
                AccountNumber = GenerateAccountNumber(),
                Balance = 0,
                Currency = request.Currency,
                AccountType = request.AccountType,
                OpenedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            await _dbRepository.AddAsync(personal);
            return await MapToAccountDto(personal);
        }

        public async Task<decimal> GetBalanceAsync(Guid accountId, Guid actingUserId)
        {
            var account = await _dbRepository.GetByIdAsync<Account>(accountId);
            await EnsureAccessAsync(actingUserId, account);
            return account!.Balance;
        }

        public async Task<List<OperationDto>> GetAccountHistoryAsync(Guid accountId, OperationFilterDto filter, Guid actingUserId)
        {
            var account = await _dbRepository.GetByIdAsync<Account>(accountId);
            await EnsureAccessAsync(actingUserId, account);

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
                result = result.Take(filter.Limit.Value).ToList();

            return result;
        }

        public async Task<OperationDto> DepositAsync(Guid accountId, decimal amount, Guid actingUserId, string description = "Пополнение счета")
        {
            if (amount <= 0)
                throw new BusinessException("Сумма должна быть больше 0");

            var account = await _dbRepository.GetByIdAsync<Account>(accountId);
            await EnsureAccessAsync(actingUserId, account);

            var operationType = await _operationTypeRepository.GetByNameAsync("DEPOSIT");
            if (operationType == null)
                throw new BusinessException("Тип операции не найден");

            var operation = new AccountOperation
            {
                Id = Guid.NewGuid(),
                ToAccountId = account!.Id,
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

        public async Task<OperationDto> WithdrawAsync(Guid accountId, decimal amount, Guid actingUserId, string description = "Снятие со счета")
        {
            if (amount <= 0)
                throw new BusinessException("Сумма должна быть больше 0");

            var account = await _dbRepository.GetByIdAsync<Account>(accountId);
            await EnsureAccessAsync(actingUserId, account);

            if (account!.Balance < amount)
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

        public async Task<TransferRecipientPreviewDto?> LookupTransferRecipientAsync(string? accountNumberRaw)
        {
            var normalized = AccountNumberNormalizer.NormalizeAccountNumber(accountNumberRaw);
            if (string.IsNullOrEmpty(normalized)) return null;
            var account = await _accountRepository.GetByAccountNumberAsync(normalized);
            if (account == null) return null;

            if (account.OrganizationId.HasValue)
            {
                var org = await _dbRepository.GetByIdAsync<Organization>(account.OrganizationId.Value);
                return new TransferRecipientPreviewDto
                {
                    AccountNumber = account.AccountNumber,
                    Currency = account.Currency,
                    AccountType = account.AccountType,
                    RecipientKind = "organization",
                    DisplayName = org?.Name ?? "Организация",
                    Inn = org?.Inn,
                };
            }

            if (account.UserId.HasValue)
            {
                var user = await _dbRepository.GetByIdAsync<User>(account.UserId.Value);
                return new TransferRecipientPreviewDto
                {
                    AccountNumber = account.AccountNumber,
                    Currency = account.Currency,
                    AccountType = account.AccountType,
                    RecipientKind = "individual",
                    DisplayName = user?.FullName ?? "Физическое лицо",
                    Inn = null,
                };
            }

            return new TransferRecipientPreviewDto
            {
                AccountNumber = account.AccountNumber,
                Currency = account.Currency,
                AccountType = account.AccountType,
                RecipientKind = "unknown",
                DisplayName = "Счёт",
                Inn = null,
            };
        }
    }
}
