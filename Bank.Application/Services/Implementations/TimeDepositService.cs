using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bank.Application.DTOs.Accounts;
using Bank.Application.DTOs.Deposits;
using Bank.Application.Exceptions;
using Bank.Application.Services.Interfaces;
using Bank.Domain.Interfaces;
using Bank.Domain.Models;

namespace Bank.Application.Services.Implementations
{
    public class TimeDepositService : ITimeDepositService
    {
        private readonly IDataBaseRepository _db;
        private readonly IAccountRepository _accounts;
        private readonly IOperationService _operations;
        private readonly IAccountService _accountService;
        private readonly IOrganizationService _organizations;

        public TimeDepositService(
            IDataBaseRepository db,
            IAccountRepository accounts,
            IOperationService operations,
            IAccountService accountService,
            IOrganizationService organizations)
        {
            _db = db;
            _accounts = accounts;
            _operations = operations;
            _accountService = accountService;
            _organizations = organizations;
        }

        private static decimal AnnualRateForTerm(int months) => months switch
        {
            <= 3 => 5.5m,
            <= 6 => 7.0m,
            _ => 9.5m
        };

        private static void EnsureSameOwner(Account from, OpenTimeDepositRequest request)
        {
            if (request.OrganizationId.HasValue)
            {
                if (from.OrganizationId != request.OrganizationId)
                    throw new BusinessException("Счёт не принадлежит указанной организации");
            }
            else if (from.UserId == null)
                throw new BusinessException("Для корпоративного вклада укажите organizationId");
        }

        public async Task<TimeDepositDto> OpenAsync(OpenTimeDepositRequest request, Guid actingUserId)
        {
            if (request.Amount <= 0) throw new BusinessException("Сумма должна быть больше 0");
            if (request.TermMonths is not (3 or 6 or 12))
                throw new BusinessException("Допустимые сроки вклада: 3, 6 или 12 месяцев");

            var from = await _db.GetByIdAsync<Account>(request.FromAccountId);
            if (from == null) throw new NotFoundException("Счёт не найден");

            EnsureSameOwner(from, request);

            if (request.OrganizationId.HasValue)
            {
                if (!await _organizations.UserIsMemberAsync(request.OrganizationId.Value, actingUserId))
                    throw new BusinessException("Нет доступа к организации");
            }
            else
            {
                if (from.UserId != actingUserId)
                    throw new BusinessException("Нет доступа к счёту");
            }

            var rate = AnnualRateForTerm(request.TermMonths);
            var maturity = DateTime.UtcNow.AddMonths(request.TermMonths);

            var depositAccount = new Account
            {
                Id = Guid.NewGuid(),
                UserId = from.UserId,
                OrganizationId = from.OrganizationId,
                AccountNumber = "9112" + DateTime.UtcNow.Ticks.ToString().Substring(0, 10),
                Balance = 0,
                Currency = from.Currency,
                AccountType = "time_deposit",
                OpenedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            await _db.AddAsync(depositAccount);

            await _operations.TransferAsync(new TransferRequest
            {
                FromAccountId = from.Id,
                ToAccountId = depositAccount.Id,
                Amount = request.Amount,
                Description = "Размещение срочного вклада"
            }, actingUserId);

            var td = new TimeDeposit
            {
                Id = Guid.NewGuid(),
                UserId = from.UserId,
                OrganizationId = from.OrganizationId,
                DepositAccountId = depositAccount.Id,
                Principal = request.Amount,
                AnnualRatePercent = rate,
                TermMonths = request.TermMonths,
                OpenedAt = DateTime.UtcNow,
                MaturityDate = maturity,
                Status = "Active"
            };
            await _db.AddAsync(td);

            return Map(td, depositAccount.AccountNumber);
        }

        public async Task<List<TimeDepositDto>> ListForUserAsync(Guid userId)
        {
            var all = await _db.GetAllAsync<TimeDeposit>();
            var list = all.Where(t => t.UserId == userId && t.OrganizationId == null).ToList();
            return await MapList(list);
        }

        public async Task<List<TimeDepositDto>> ListForOrganizationAsync(Guid organizationId, Guid actingUserId)
        {
            if (!await _organizations.UserIsMemberAsync(organizationId, actingUserId))
                throw new BusinessException("Нет доступа к организации");

            var all = await _db.GetAllAsync<TimeDeposit>();
            var list = all.Where(t => t.OrganizationId == organizationId).ToList();
            return await MapList(list);
        }

        public async Task<TimeDepositDto> CloseAsync(CloseTimeDepositRequest request, Guid actingUserId)
        {
            var td = await _db.GetByIdAsync<TimeDeposit>(request.TimeDepositId);
            if (td == null) throw new NotFoundException("Вклад не найден");
            if (td.Status != "Active") throw new BusinessException("Вклад уже закрыт");

            var depositAcc = await _db.GetByIdAsync<Account>(td.DepositAccountId);
            if (depositAcc == null) throw new NotFoundException("Счёт вклада не найден");

            if (td.OrganizationId.HasValue)
            {
                if (!await _organizations.UserIsMemberAsync(td.OrganizationId.Value, actingUserId))
                    throw new BusinessException("Нет доступа");
            }
            else if (td.UserId != actingUserId)
                throw new BusinessException("Нет доступа");

            var target = await _db.GetByIdAsync<Account>(request.TargetAccountId);
            if (target == null) throw new NotFoundException("Счёт зачисления не найден");

            if (depositAcc.Currency != target.Currency)
                throw new BusinessException("Валюты должны совпадать");

            if (td.OrganizationId == null)
            {
                if (target.UserId != td.UserId)
                    throw new BusinessException("Вклад физлица можно закрыть только на свой счёт");
            }
            else
            {
                if (target.OrganizationId != td.OrganizationId)
                    throw new BusinessException("Корпоративный вклад закрывается на счёт той же организации");
            }

            var monthsElapsed = Math.Max(1, (int)((DateTime.UtcNow - td.OpenedAt).TotalDays / 30));
            var effectiveMonths = Math.Min(td.TermMonths, monthsElapsed);
            var interest = Math.Round(td.Principal * td.AnnualRatePercent / 100m * effectiveMonths / 12m, 2, MidpointRounding.AwayFromZero);

            await _accountService.DepositAsync(td.DepositAccountId, interest, actingUserId, "Начисление процентов по вкладу");

            depositAcc = await _db.GetByIdAsync<Account>(td.DepositAccountId);
            if (depositAcc == null) throw new NotFoundException("Счёт вклада не найден");

            var total = depositAcc.Balance;
            if (total <= 0) throw new BusinessException("Нечего переводить");

            await _operations.TransferAsync(new TransferRequest
            {
                FromAccountId = td.DepositAccountId,
                ToAccountId = request.TargetAccountId,
                Amount = total,
                Description = "Возврат вклада и процентов"
            }, actingUserId);

            td.Status = "Closed";
            await _db.UpdateAsync(td);

            depositAcc = await _db.GetByIdAsync<Account>(td.DepositAccountId);
            if (depositAcc != null && depositAcc.Balance > 0)
            {
                
                await _accountService.DepositAsync(request.TargetAccountId, depositAcc.Balance, actingUserId, "Остаток по вкладу");
                depositAcc.Balance = 0;
                await _db.UpdateAsync(depositAcc);
            }

            return Map(td, (await _db.GetByIdAsync<Account>(td.DepositAccountId))?.AccountNumber ?? "");
        }

        private async Task<List<TimeDepositDto>> MapList(List<TimeDeposit> list)
        {
            var result = new List<TimeDepositDto>();
            foreach (var t in list.OrderByDescending(x => x.OpenedAt))
            {
                var acc = await _db.GetByIdAsync<Account>(t.DepositAccountId);
                result.Add(Map(t, acc?.AccountNumber ?? ""));
            }
            return result;
        }

        private static TimeDepositDto Map(TimeDeposit t, string accNum) => new()
        {
            Id = t.Id,
            UserId = t.UserId,
            OrganizationId = t.OrganizationId,
            DepositAccountId = t.DepositAccountId,
            DepositAccountNumber = accNum,
            Principal = t.Principal,
            AnnualRatePercent = t.AnnualRatePercent,
            TermMonths = t.TermMonths,
            OpenedAt = t.OpenedAt,
            MaturityDate = t.MaturityDate,
            Status = t.Status
        };
    }
}
