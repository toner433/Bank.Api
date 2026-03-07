using Bank.Application.DTOs.Accounts;
using Bank.Application.DTOs.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountDto?> GetByIdAsync(Guid id);
        Task<List<AccountDto>> GetByUserIdAsync(Guid userId);
        Task<AccountDto> CreateAccountAsync(CreateAccountRequest request);
        Task<decimal> GetBalanceAsync(Guid accountId);
        Task<List<OperationDto>> GetAccountHistoryAsync(Guid accountId, OperationFilterDto filter);
        Task<OperationDto> DepositAsync(Guid accountId, decimal amount, string description = "Пополнение счета");
        Task<OperationDto> WithdrawAsync(Guid accountId, decimal amount, string description = "Снятие со счета");
    }
}
