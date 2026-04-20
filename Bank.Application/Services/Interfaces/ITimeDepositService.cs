using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bank.Application.DTOs.Deposits;

namespace Bank.Application.Services.Interfaces
{
    public interface ITimeDepositService
    {
        Task<TimeDepositDto> OpenAsync(OpenTimeDepositRequest request, Guid actingUserId);
        Task<List<TimeDepositDto>> ListForUserAsync(Guid userId);
        Task<List<TimeDepositDto>> ListForOrganizationAsync(Guid organizationId, Guid actingUserId);
        Task<TimeDepositDto> CloseAsync(CloseTimeDepositRequest request, Guid actingUserId);
    }
}
