using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bank.Application.DTOs.Accounts;
using Bank.Application.DTOs.Operations;

namespace Bank.Application.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountDto?> GetByIdAsync(Guid id, Guid actingUserId);
        Task<List<AccountDto>> GetByUserIdAsync(Guid userId, Guid actingUserId);
        Task<List<AccountDto>> GetAccessibleAccountsAsync(Guid actingUserId);
        Task<AccountDto> CreateAccountAsync(CreateAccountRequest request, Guid actingUserId);
        Task<decimal> GetBalanceAsync(Guid accountId, Guid actingUserId);
        Task<List<OperationDto>> GetAccountHistoryAsync(Guid accountId, OperationFilterDto filter, Guid actingUserId);
        Task<OperationDto> DepositAsync(Guid accountId, decimal amount, Guid actingUserId, string description = "Пополнение счета");
        Task<OperationDto> WithdrawAsync(Guid accountId, decimal amount, Guid actingUserId, string description = "Снятие со счета");
        Task<TransferRecipientPreviewDto?> LookupTransferRecipientAsync(string? accountNumber);
    }
}
