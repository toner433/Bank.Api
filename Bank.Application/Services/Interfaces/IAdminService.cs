using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bank.Application.DTOs.Admin;

namespace Bank.Application.Services.Interfaces
{
    public interface IAdminService
    {
        Task EnsureAdminAsync(Guid actingUserId);
        Task<AdminStatsDto> GetStatsAsync(Guid actingUserId);
        Task<List<AdminUserListItemDto>> ListUsersAsync(Guid actingUserId);
        Task<List<AdminOrganizationListItemDto>> ListOrganizationsAsync(Guid actingUserId);
        Task<List<AdminAccountListItemDto>> ListAccountsAsync(Guid actingUserId);
        Task SetUserBlockedAsync(Guid actingUserId, Guid targetUserId, bool blocked);
        Task SetAccountBlockedAsync(Guid actingUserId, Guid accountId, bool blocked);
        Task UpdateAccountAsync(Guid actingUserId, Guid accountId, UpdateAdminAccountRequest request);
    }
}
